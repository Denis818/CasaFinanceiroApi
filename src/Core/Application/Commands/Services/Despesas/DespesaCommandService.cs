using Application.Commands.Dtos;
using Application.Commands.Interfaces;
using Application.Commands.Services.Base;
using Application.Configurations.MappingsApp;
using Application.Resources.Messages;
using Domain.Converters.DatesTimes;
using Domain.Enumeradores;
using Domain.Extensions.Help;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Repositories.Categorias;
using Domain.Interfaces.Repositories.GrupoFaturas;
using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands.Services
{
    public class DespesaCommandService(
        IServiceProvider service,
        IGrupoFaturaRepository _grupoFaturaRepository,
        ICategoriaRepository _categoriaRepository
    )
        : BaseCommandService<Despesa, DespesaCommandDto, IDespesaRepository>(service),
            IDespesaCommandService
    {
        protected override Despesa MapToEntity(DespesaCommandDto entity) => entity.MapToEntity();

        public async Task<bool> InsertAsync(DespesaCommandDto despesaDto)
        {
            if (Validator(despesaDto))
                return false;

            (bool despesaIsValid, despesaDto) = await ValidarDespesaAsync(despesaDto);

            if (!despesaIsValid)
                return false;

            var despesa = despesaDto.MapToEntity();

            despesa.Total = (despesa.Preco * despesa.Quantidade).RoundTo(2);
            despesa.DataCompra = DateTimeZoneConverterPtBR.GetBrasiliaDateTimeZone();

            await _repository.InsertAsync(despesa);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Inserir")
                );
                return false;
            }

            return true;
        }

        public async Task<bool> InsertRangeAsync(
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

                if (await _categoriaRepository.ExisteAsync(despesaDto.CategoriaCode) is null)
                {
                    Notificar(
                        EnumTipoNotificacao.NotFount,
                        string.Format(Message.NaoEncontrado, "A categoria")
                    );
                    continue;
                }

                var despesa = despesaDto.MapToEntity();
                despesa.Total = (despesa.Preco * despesa.Quantidade).RoundTo(2);

                despesa.DataCompra = DateTimeZoneConverterPtBR.GetBrasiliaDateTimeZone();
                despesasParaInserir.Add(despesa);
            }

            if (despesasParaInserir.Count == 0)
            {
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    "Nunhuma das despesa é valida para inserir."
                );
                return false;
            }

            await _repository.InsertRangeAsync(despesasParaInserir);
            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Inserir")
                );
                return false;
            }

            if (totalRecebido > despesasParaInserir.Count)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    $"{despesasParaInserir.Count} de {totalRecebido} despesas foram inseridas. "
                        + $"total de {totalRecebido - despesasParaInserir.Count} invalidas."
                );
            }

            return true;
        }

        public async Task<bool> UpdateAsync(Guid code, DespesaCommandDto despesaDto)
        {
            if (Validator(despesaDto))
                return false;

            var despesa = await GetDespesaByCodigoAsync(code);
            if (despesa == null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.NaoEncontrado, "A despesa")
                );
                return false;
            }

            (bool despesaIsValid, despesaDto) = await ValidarDespesaAsync(despesaDto, code);

            if (!despesaIsValid)
                return false;

            despesa.MapUpdateEntity(despesaDto);

            despesa.GrupoFatura = await _grupoFaturaRepository.GetByCodigoAsync(
                despesaDto.GrupoFaturaCode
            );

            despesa.Total = despesa.Preco * despesa.Quantidade;
            despesa.DataCompra = DateTimeZoneConverterPtBR.GetBrasiliaDateTimeZone();

            _repository.Update(despesa);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Atualizar")
                );
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteAsync(Guid code)
        {
            var despesa = await _repository.GetByCodigoAsync(code);

            if (despesa == null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.NaoEncontrado, "A despesa")
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

        private async Task<(bool, DespesaCommandDto)> ValidarDespesaAsync(
            DespesaCommandDto despesaDto,
            Guid? codeDespesaInEdicao = null
        )
        {
            var categoria = await _categoriaRepository.ExisteAsync(despesaDto.CategoriaCode);
            var grupoFatura = await _grupoFaturaRepository.ExisteAsync(despesaDto.GrupoFaturaCode);

            if (categoria is null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.NaoEncontrado, "A categoria")
                );
                return (false, null);
            }

            if (grupoFatura is null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.NaoEncontrado, "O Grupo de Despesa")
                );
                return (false, null);
            }

            despesaDto.CategoriaId = categoria.Id;
            despesaDto.GrupoFaturaId = grupoFatura.Id;

            if (
                despesaDto.CategoriaCode == CategoriaCods.CodAluguel
                && !despesaDto.Item.ToLower().Contains("caixa")
                && !despesaDto.Item.ToLower().Contains("parcela ap ponto")
            )
            {
                Notificar(EnumTipoNotificacao.Informacao, Message.CadastroAluguelIncorreto);
                return (false, null);
            }

            if (
                despesaDto.CategoriaCode == CategoriaCods.CodCondominio
                && !despesaDto.Item.Contains(
                    "condomínio ap ponto",
                    StringComparison.CurrentCultureIgnoreCase
                )
            )
            {
                Notificar(EnumTipoNotificacao.Informacao, Message.CadastroCondominioIncorreto);
                return (false, null);
            }

            if (!await IsDespesaMensalExistenteAsync(despesaDto, codeDespesaInEdicao))
            {
                return (false, null);
            }

            return (true, despesaDto);
        }

        private async Task<bool> IsDespesaMensalExistenteAsync(
            DespesaCommandDto despesaDto,
            Guid? codeDespesaInEdicao
        )
        {
            if (!EhDespesaMensal(despesaDto.CategoriaCode))
            {
                return true;
            }

            if (despesaDto.Quantidade != 1)
            {
                Notificar(EnumTipoNotificacao.Informacao, Message.DespesaMensalQuantidadeDeveSerUm);
                return false;
            }

            var despesasExistentes = _repository.Get(d =>
                d.GrupoFatura.Code == despesaDto.GrupoFaturaCode
                && d.Categoria.Code == despesaDto.CategoriaCode
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
                    despesa.Categoria.Code == CategoriaCods.CodAluguel
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
                else if (despesa.Categoria.Code != CategoriaCods.CodAluguel)
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
            return codeCategoria == CategoriaCods.CodAluguel
                || codeCategoria == CategoriaCods.CodCondominio
                || codeCategoria == CategoriaCods.CodContaDeLuz;
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
