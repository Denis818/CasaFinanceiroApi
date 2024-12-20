﻿using Application.Configurations.MappingsApp;
using Application.Queries.Dtos;
using Application.Queries.Interfaces.Despesas;
using Application.Queries.Interfaces.Telas;
using Application.Queries.Services.Base;
using Application.Resources.Messages;
using Application.Utilities;
using Domain.Dtos;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Domain.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Services.Telas
{
    public class AuditoriaComprasQueryServices(
        IServiceProvider service,
        IDespesaFiltroService despesaFiltro
    )
        : BaseQueryService<Despesa, DespesaDto, IDespesaRepository>(service),
            IAuditoriaComprasQueryServices
    {
        protected override DespesaDto MapToDTO(Despesa entity) => entity.MapToDTO();

        #region Auditoria de Compras
        public async Task<PagedResult<DespesaDto>> GetListDespesasAllGroups(
            DespesaFiltroDto despesaFiltroDto,
            string ano
        )
        {
            var queryDespesasAllGrupo = _repository
                .Get(despesa => despesa.GrupoFatura.Ano == ano)
                .Include(c => c.Categoria)
                .Include(c => c.GrupoFatura);

            if (string.IsNullOrEmpty(despesaFiltroDto.Filter))
            {
                return await GetAllDespesas(
                    queryDespesasAllGrupo,
                    despesaFiltroDto.PaginaAtual,
                    despesaFiltroDto.ItensPorPagina
                );
            }

            var query = despesaFiltro.GetDespesasFiltradas(
                queryDespesasAllGrupo,
                despesaFiltroDto.Filter,
                despesaFiltroDto.TipoFiltro
            );

            var listaPaginada = await Pagination.PaginateResultAsync(
                query.Select(d => d.MapToDTO()),
                despesaFiltroDto.PaginaAtual,
                despesaFiltroDto.ItensPorPagina
            );

            if (listaPaginada.Itens.Count == 0)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    "Não há despesa em nenhum grupo de fatura"
                );

                listaPaginada.Itens = [];

                return listaPaginada;
            }

            return listaPaginada;
        }

        public IEnumerable<DespesasSugestaoEconomiaQueryDto> GetSugestoesEconomiaGrafico()
        {
            var list = ListDespesasPorGrupo
                .Where(d =>
                    d.Categoria.Code != CategoriaCods.CodAluguel
                    && d.Categoria.Code != CategoriaCods.CodCondominio
                    && d.Categoria.Code != CategoriaCods.CodContaDeLuz
                    && !d.Item.ToLower().Contains("compra")
                );

            var sugestoes = list.GroupBy(d => NormalizeItemName(d.Item))
                .Where(g => g.Select(d => d.Fornecedor).Distinct().Count() > 1)
                .Select(group => new DespesasSugestaoEconomiaQueryDto
                {
                    Item = group.Key,
                    FornecedorMaisBarato = group.OrderBy(d => d.Preco).First().Fornecedor,
                    PrecoMaisBarato = group.Min(d => d.Preco),
                    PotencialEconomia = group.Max(d => d.Preco) - group.Min(d => d.Preco)
                })
                .Where(s => s.PotencialEconomia > 0);

            if (!sugestoes.Any())
            {
                Notificar(EnumTipoNotificacao.Informacao, "Não há sugestões.");

                return [];
            }

            return sugestoes;
        }

        public async Task<
            IEnumerable<DespesasSugestaoDeFornecedorQueryDto>
        > SugestaoDeFornecedorMaisBarato(int paginaAtual, int itensPorPagina)
        {
            List<DespesasSugestaoDeFornecedorQueryDto> sugestoes = [];
            var categorias = await _categoriaRepository.Get().ToListAsync();

            var despesasSomenteCasa = ListDespesasPorGrupo
                .Where(d =>
                    d.Categoria.Code != CategoriaCods.CodAluguel
                    && d.Categoria.Code != CategoriaCods.CodCondominio
                    && d.Categoria.Code != CategoriaCods.CodContaDeLuz
                    && !d.Item.ToLower().Contains("compra")
                )
                .OrderByDescending(d => d.DataCompra);

            foreach (var categoria in categorias)
            {
                var despesasPorCategoria = despesasSomenteCasa.Where(d =>
                    d.CategoriaCode == categoria.Code
                );

                var itensAgrupados = despesasPorCategoria.GroupBy(d => NormalizeItemName(d.Item));

                foreach (var grupoItem in itensAgrupados)
                {
                    if (
                        grupoItem.Count() <= 1
                        || grupoItem.Select(d => d.Fornecedor).Distinct().Count() <= 1
                    )
                    {
                        continue;
                    }

                    var fornecedorMaisBarato = grupoItem.OrderBy(d => d.Preco).First();

                    var listaPaginada = Pagination.PaginateResult(
                        grupoItem.Select(d => d.MapToDTO()).ToList(),
                        paginaAtual,
                        itensPorPagina
                    );

                    sugestoes.Add(
                        new DespesasSugestaoDeFornecedorQueryDto
                        {
                            Sugestao =
                                $"{grupoItem.Key} em {fornecedorMaisBarato.Fornecedor} é mais barato",
                            ListaItens = listaPaginada
                        }
                    );
                }
            }

            if (sugestoes.Count == 0)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    "Nenhuma sugestão de otimização disponível no momento."
                );
            }

            return sugestoes;
        }

        #endregion

        private async Task<PagedResult<DespesaDto>> GetAllDespesas(
            IQueryable<Despesa> query,
            int paginaAtual,
            int itensPorPagina
        )
        {
            var queryAll = query
                .Include(c => c.Categoria)
                .Include(c => c.GrupoFatura)
                .OrderByDescending(d => d.DataCompra)
                .Select(d => d.MapToDTO());

            var despesas = await Pagination.PaginateResultAsync(
                queryAll,
                paginaAtual,
                itensPorPagina
            );

            if (despesas.TotalItens == 0)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    string.Format(Message.DespesasNaoEncontradas, "")
                );
            }

            return despesas;
        }

        private string NormalizeItemName(string itemName)
        {
            var words = itemName.ToLower().Split(' ');
            return words[0];
        }
    }
}
