using Application.Commands.Dtos;
using Application.Commands.Interfaces;
using Application.Commands.Services.Base;
using Application.Configurations.MappingsApp;
using Application.Resources.Messages;
using Domain.Converters.DatesTimes;
using Domain.Dtos;
using Domain.Enumeradores;
using Domain.Extensions.Help;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands.Services
{
    public class DespesaCommandService(
        IServiceProvider service,
        IGrupoFaturaRepository _GrupoFaturaRepository,
        ICategoriaRepository _categoriaRepository
    )
        : BaseCommandService<Despesa, DespesaCommandDto, IDespesaRepository>(service),
            IDespesaCommandService
    {
        private readonly CategoriaCodsDto _categoriaIds = _categoriaRepository.GetCategoriaCodes();

        protected override Despesa MapToEntity(DespesaCommandDto entity) => entity.MapToEntity();

        public async Task InsertAsync(DespesaCommandDto despesaDto)
        {
            if (Validator(despesaDto))
                return;

            if (!await ValidarDespesaAsync(despesaDto))
                return;

            var despesa = despesaDto.MapToEntity();

            despesa.Total = (despesa.Preco * despesa.Quantidade).RoundTo(2);
            despesa.DataCompra = DateTimeZoneProvider.GetBrasiliaDateTimeZone();

            await _repository.InsertAsync(despesa);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Inserir")
                );
                return;
            }
        }

        public async Task InsertRangeAsync(
            IAsyncEnumerable<DespesaCommandDto> listDespesasDto
        )
        {
            int totalRecebido = 0;
            var despesasParaInserir = new List<Despesa>();

            await foreach (var despesaDto in listDespesasDto)
            {
                totalRecebido++;

                if (Validator(despesaDto))
                    continue;

                if (await _categoriaRepository.ExisteAsync(despesaDto.Categoria.Code) is null)
                {
                    Notificar(
                        EnumTipoNotificacao.NotFount,
                        string.Format(
                            Message.IdNaoEncontrado,
                            "A categoria",
                            despesaDto.Categoria.Code
                        )
                    );
                    continue;
                }

                var despesa = despesaDto.MapToEntity();
                despesa.Total = (despesa.Preco * despesa.Quantidade).RoundTo(2);

                despesa.DataCompra = DateTimeZoneProvider.GetBrasiliaDateTimeZone();
                despesasParaInserir.Add(despesa);
            }

            if (despesasParaInserir.Count == 0)
            {
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    "Nunhuma das despesa é valida para inserir."
                );
                return;
            }

            await _repository.InsertRangeAsync(despesasParaInserir);
            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Inserir")
                );
                return;
            }

            if (totalRecebido > despesasParaInserir.Count)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    $"{despesasParaInserir.Count} de {totalRecebido} despesas foram inseridas. "
                        + $"total de {totalRecebido - despesasParaInserir.Count} invalidas."
                );
            }
        }

        public async Task UpdateAsync(Guid code, DespesaCommandDto despesaDto)
        {
            if (Validator(despesaDto))
                return;

            var despesa = await GetDespesaByCodigoAsync(code);
            if (despesa == null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.IdNaoEncontrado, "A despesa", code)
                );
                return;
            }

            if (!await ValidarDespesaAsync(despesaDto, code))
                return;

            despesa.MapUpdateEntity(despesaDto);

            despesa.Total = despesa.Preco * despesa.Quantidade;
            despesa.DataCompra = DateTimeZoneProvider.GetBrasiliaDateTimeZone();

            _repository.Update(despesa);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Atualizar")
                );
                return;
            }
        }

        public async Task<bool> DeleteAsync(Guid code)
        {
            var despesa = await _repository.GetByCodigoAsync(code);

            if (despesa == null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.IdNaoEncontrado, "A despesa", code)
                );
                return false;
            }

            _repository.Delete(despesa);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Deletar")
                );
                return false;
            }

            return true;
        }

        #region Metodos de Suporte

        private async Task<bool> ValidarDespesaAsync(
            DespesaCommandDto despesaDto,
            Guid? codeDespesaInEdicao = null
        )
        {
            if (await _categoriaRepository.ExisteAsync(despesaDto.Categoria.Code) is null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.IdNaoEncontrado, "A categoria", despesaDto.Categoria.Code)
                );
                return false;
            }

            if (
                despesaDto.Categoria.Code == _categoriaIds.CodAluguel
                && !despesaDto.Item.ToLower().Contains("caixa")
                && !despesaDto.Item.ToLower().Contains("parcela ap ponto")
            )
            {
                Notificar(EnumTipoNotificacao.Informacao, Message.CadastroAluguelIncorreto);
                return false;
            }

            if (
                despesaDto.Categoria.Code == _categoriaIds.CodCondominio
                && !despesaDto.Item.Contains(
                    "condomínio ap ponto",
                    StringComparison.CurrentCultureIgnoreCase
                )
            )
            {
                Notificar(EnumTipoNotificacao.Informacao, Message.CadastroCondominioIncorreto);
                return false;
            }

            if (await _GrupoFaturaRepository.ExisteAsync(despesaDto.GrupoFatura.Code) is null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(
                        Message.IdNaoEncontrado,
                        "O Grupo de Despesa",
                        despesaDto.GrupoFatura.Code
                    )
                );
                return false;
            }

            if (!await IsDespesaMensalExistenteAsync(despesaDto, codeDespesaInEdicao))
            {
                return false;
            }

            return true;
        }

        private async Task<bool> IsDespesaMensalExistenteAsync(
            DespesaCommandDto despesaDto,
            Guid? codeDespesaInEdicao
        )
        {
            if (!EhDespesaMensal(despesaDto.Categoria.Code))
            {
                return true;
            }

            if (despesaDto.Quantidade != 1)
            {
                Notificar(EnumTipoNotificacao.Informacao, Message.DespesaMensalQuantidadeDeveSerUm);
                return false;
            }

            var despesasExistentes = _repository.Get(d =>
                d.GrupoFatura.Code == despesaDto.GrupoFatura.Code
                && d.Categoria.Code == despesaDto.Categoria.Code
            );

            if (codeDespesaInEdicao != null)
            {
                despesasExistentes = despesasExistentes.Where(despesa =>
                    despesa.Code != codeDespesaInEdicao
                );
            }

            var listExistentes = await despesasExistentes.ToListAsync();
            foreach (var despesa in listExistentes)
            {
                if (
                    despesa.Categoria.Code == _categoriaIds.CodAluguel
                    && despesa.Item.Equals(despesaDto.Item, StringComparison.OrdinalIgnoreCase)
                )
                {
                    Notificar(
                        EnumTipoNotificacao.Informacao,
                        string.Format(
                            Message.DespesaExistente,
                            $"{despesa.Categoria.Descricao} {despesa.Item}"
                        )
                    );
                    return false;
                }
                else if (despesa.Categoria.Code != _categoriaIds.CodAluguel)
                {
                    Notificar(
                        EnumTipoNotificacao.Informacao,
                        string.Format(Message.DespesaExistente, despesa.Categoria.Descricao)
                    );

                    return false;
                }

                return true;
            }

            return true;
        }

        private bool EhDespesaMensal(Guid codeCategoria)
        {
            return codeCategoria == _categoriaIds.CodAluguel
                || codeCategoria == _categoriaIds.CodCondominio
                || codeCategoria == _categoriaIds.CodContaDeLuz
                || codeCategoria == _categoriaIds.CodInternet;
        }

        private async Task<Despesa> GetDespesaByCodigoAsync(Guid code)
        {
            var despesa = await _repository
                .Get(despesa => despesa.Code == code)
                .Include(x => x.Categoria)
                .Include(x => x.GrupoFatura)
                .FirstOrDefaultAsync();

            return despesa;
        }

        #endregion
    }
}
