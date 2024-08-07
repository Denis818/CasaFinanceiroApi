﻿using Application.Commands.Dtos;
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
    ) : BaseCommandService<Despesa, IDespesaRepository>(service), IDespesaCommandService
    {
        private readonly CategoriaIdsDto _categoriaIds = _categoriaRepository.GetCategoriaIds();

        public async Task<Despesa> InsertAsync(DespesaCommandDto despesaDto)
        {
            if (Validator(despesaDto))
                return null;

            if (!await ValidarDespesaAsync(despesaDto))
                return null;

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
                return null;
            }

            return await GetByIdAsync(despesa.Id);
        }

        public async Task<IEnumerable<Despesa>> InsertRangeAsync(
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

                if (await _categoriaRepository.ExisteAsync(despesaDto.CategoriaId) is null)
                {
                    Notificar(
                        EnumTipoNotificacao.NotFount,
                        string.Format(
                            Message.IdNaoEncontrado,
                            "A categoria",
                            despesaDto.CategoriaId
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
                return null;
            }

            await _repository.InsertRangeAsync(despesasParaInserir);
            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Inserir")
                );
                return null;
            }

            if (totalRecebido > despesasParaInserir.Count)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    $"{despesasParaInserir.Count} de {totalRecebido} despesas foram inseridas. "
                        + $"total de {totalRecebido - despesasParaInserir.Count} invalidas."
                );
            }

            var ids = despesasParaInserir.Select(d => d.Id).ToList();
            var despesasInseridas = await _repository
                .Get(d => ids.Contains(d.Id))
                .Include(c => c.Categoria)
                .Include(c => c.GrupoFatura)
                .AsNoTracking()
                .ToListAsync();

            return despesasInseridas;
        }

        public async Task<Despesa> UpdateAsync(int id, DespesaCommandDto despesaDto)
        {
            if (Validator(despesaDto))
                return null;

            var despesa = await _repository.GetByIdAsync(id);
            if (despesa == null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.IdNaoEncontrado, "A despesa", id)
                );
                return null;
            }

            if (!await ValidarDespesaAsync(despesaDto, id))
                return null;

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
                return null;
            }

            return await GetByIdAsync(despesa.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var despesa = await _repository.GetByIdAsync(id);

            if (despesa == null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.IdNaoEncontrado, "A despesa", id)
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
            int idDespesaInEdicao = 0
        )
        {
            if (await _categoriaRepository.ExisteAsync(despesaDto.CategoriaId) is null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.IdNaoEncontrado, "A categoria", despesaDto.CategoriaId)
                );
                return false;
            }

            if (
                despesaDto.CategoriaId == _categoriaIds.IdAluguel
                && !despesaDto.Item.ToLower().Contains("caixa")
                && !despesaDto.Item.ToLower().Contains("parcela ap ponto")
            )
            {
                Notificar(EnumTipoNotificacao.Informacao, Message.CadastroAluguelIncorreto);
                return false;
            }

            if (
                despesaDto.CategoriaId == _categoriaIds.IdCondominio
                && !despesaDto.Item.Contains(
                    "condomínio ap ponto",
                    StringComparison.CurrentCultureIgnoreCase
                )
            )
            {
                Notificar(EnumTipoNotificacao.Informacao, Message.CadastroCondominioIncorreto);
                return false;
            }

            if (await _GrupoFaturaRepository.ExisteAsync(despesaDto.GrupoFaturaId) is null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(
                        Message.IdNaoEncontrado,
                        "O Grupo de Despesa",
                        despesaDto.GrupoFaturaId
                    )
                );
                return false;
            }

            if (!await IsDespesaMensalExistenteAsync(despesaDto, idDespesaInEdicao))
            {
                return false;
            }

            return true;
        }

        private async Task<bool> IsDespesaMensalExistenteAsync(
            DespesaCommandDto despesaDto,
            int idDespesaInEdicao
        )
        {
            if (!EhDespesaMensal(despesaDto.CategoriaId))
            {
                return true;
            }

            if (despesaDto.Quantidade != 1)
            {
                Notificar(EnumTipoNotificacao.Informacao, Message.DespesaMensalQuantidadeDeveSerUm);
                return false;
            }

            var despesasExistentes = _repository.Get(d =>
                d.GrupoFaturaId == despesaDto.GrupoFaturaId
                && d.CategoriaId == despesaDto.CategoriaId
            );

            if (idDespesaInEdicao != 0)
            {
                despesasExistentes = despesasExistentes.Where(despesa =>
                    despesa.Id != idDespesaInEdicao
                );
            }

            var listExistentes = await despesasExistentes.ToListAsync();
            foreach (var despesa in listExistentes)
            {
                if (
                    despesa.CategoriaId == _categoriaIds.IdAluguel
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
                else if (despesa.CategoriaId != _categoriaIds.IdAluguel)
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

        private bool EhDespesaMensal(int idCategoria)
        {
            return idCategoria == _categoriaIds.IdAluguel
                || idCategoria == _categoriaIds.IdCondominio
                || idCategoria == _categoriaIds.IdContaDeLuz
                || idCategoria == _categoriaIds.IdInternet;
        }

        private async Task<Despesa> GetByIdAsync(int id)
        {
            var despesa = await _repository
                .Get(despesa => despesa.Id == id)
                .Include(x => x.Categoria)
                .Include(x => x.GrupoFatura)
                .FirstOrDefaultAsync();

            return despesa;
        }

        #endregion
    }
}
