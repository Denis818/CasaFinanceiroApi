using Application.Queries.Dtos;
using Application.Queries.Interfaces;
using Application.Queries.Services.Base;
using Application.Resources.Messages;
using Application.Utilities;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Domain.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Services
{
    public class ConferenciaComprasQueryServices(IServiceProvider service)
        : BaseQueryService<Despesa, IDespesaRepository>(service), IConferenciaComprasQueryServices
    {
        #region Conferência de Compras
        public async Task<PagedResult<Despesa>> GetListDespesasAllGroups(
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

            IOrderedQueryable<Despesa> query = GetDespesasFiltradas(
                queryDespesasAllGrupo,
                despesaFiltroDto.Filter,
                despesaFiltroDto.TipoFiltro
            );

            var listaPaginada = await Pagination.PaginateResultAsync(
                query,
                despesaFiltroDto.PaginaAtual,
                despesaFiltroDto.ItensPorPagina
            );

            return listaPaginada;
        }

        public async Task<List<DespesasSugestaoEconomiaQueryDto>> GetSugestoesEconomiaPorGrupoAsync()
        {
            var sugestoes = await _queryDespesasPorGrupo
                .Where(d =>
                    d.CategoriaId != _categoriaIds.IdAluguel
                    && d.CategoriaId != _categoriaIds.IdCondominio
                    && d.CategoriaId != _categoriaIds.IdContaDeLuz
                    && d.CategoriaId != _categoriaIds.IdInternet
                )
                .GroupBy(d => d.Item)
                .Select(group => new DespesasSugestaoEconomiaQueryDto
                {
                    Item = group.Key,
                    FornecedorMaisBarato = group.OrderBy(d => d.Preco).First().Fornecedor,
                    PrecoMaisBarato = group.Min(d => d.Preco),
                    PotencialEconomia = group.Max(d => d.Preco) - group.Min(d => d.Preco)
                })
                .Where(s => s.PotencialEconomia > 0)
                .ToListAsync();

            return sugestoes;
        }

        public async Task<IEnumerable<DespesasSugestaoDeFornecedorQueryDto>> SugestaoDeFornecedorMaisBarato(int paginaAtual, int itensPorPagina)
        {
            var queryDespesasPorGrupo = _repository
               .Get(d => d.GrupoFaturaId == _grupoId)
               .Include(c => c.Categoria)
               .Include(c => c.GrupoFatura);

            var categorias = await _categoriaRepository.Get().ToListAsync();

            List<DespesasSugestaoDeFornecedorQueryDto> sugestoes = new();

            foreach (var categoria in categorias)
            {
                var despesasSomenteCasa = _queryDespesasPorGrupo
                    .Where(c =>
                        c.CategoriaId != _categoriaIds.IdAluguel
                        && c.CategoriaId != _categoriaIds.IdCondominio
                        && c.CategoriaId != _categoriaIds.IdContaDeLuz
                        && c.CategoriaId != _categoriaIds.IdInternet
                    )
                    .Include(c => c.Categoria)
                    .Include(g => g.GrupoFatura)
                    .OrderByDescending(d => d.DataCompra);

                var itensAgrupados = await despesasSomenteCasa
                    .Where(d => d.CategoriaId == categoria.Id)
                    .GroupBy(d => d.Item.ToLower())
                    .ToListAsync();

                foreach (var grupoItem in itensAgrupados)
                {
                    if (grupoItem.Count() <= 1)
                    {
                        continue;
                    }

                    var fornecedorMaisBarato = grupoItem.OrderBy(d => d.Preco).First();

                    sugestoes.Add(
                        new DespesasSugestaoDeFornecedorQueryDto
                        {
                            Sugestao =
                                $"{grupoItem.Key} em {fornecedorMaisBarato.Fornecedor} é mais barato",
                            ListaItens = Pagination.PaginateResult(
                                grupoItem.ToList(),
                                paginaAtual,
                                itensPorPagina
                            )
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

        #region Filter Despesas
        private IOrderedQueryable<Despesa> GetDespesasFiltradas(
            IQueryable<Despesa> query,
            string filter,
            EnumFiltroDespesa tipoFiltro
        )
        {
            switch (tipoFiltro)
            {
                case EnumFiltroDespesa.Item:
                query = query.Where(despesa =>
                    despesa.Item.ToLower().Contains(filter.ToLower())
                );
                break;

                case EnumFiltroDespesa.Categoria:
                query = query.Where(despesa =>
                    despesa.Categoria.Descricao.ToLower().Contains(filter.ToLower())
                );
                break;

                case EnumFiltroDespesa.Fornecedor:
                query = query.Where(despesa =>
                    despesa.Fornecedor.ToLower().Contains(filter.ToLower())
                );
                break;

                case EnumFiltroDespesa.GrupoFatura:
                query = query.Where(despesa =>
                    despesa.GrupoFatura.Nome.ToLower().Contains(filter.ToLower())
                );
                break;
            }

            return query.OrderByDescending(d => d.DataCompra);
        }

        private async Task<PagedResult<Despesa>> GetAllDespesas(
            IQueryable<Despesa> query,
            int paginaAtual,
            int itensPorPagina
        )
        {
            var queryAll = query
                .Include(c => c.Categoria)
                .Include(c => c.GrupoFatura)
                .OrderByDescending(d => d.DataCompra);

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

        #endregion
    }
}
