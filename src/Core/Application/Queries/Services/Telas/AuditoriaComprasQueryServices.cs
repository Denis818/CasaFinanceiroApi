using Application.Configurations.MappingsApp;
using Application.Queries.Dtos;
using Application.Queries.Interfaces;
using Application.Queries.Services.Base;
using Application.Resources.Messages;
using Application.Utilities;
using Domain.Dtos;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Domain.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Application.Queries.Services.Telas
{
    public class AuditoriaComprasQueryServices(IServiceProvider service)
        : BaseQueryService<Despesa, DespesaQueryDto, IDespesaRepository>(service),
            IAuditoriaComprasQueryServices
    {
        protected override DespesaQueryDto MapToDTO(Despesa entity) => entity.MapToDTO();

        #region Auditoria de Compras
        public async Task<PagedResult<DespesaQueryDto>> GetListDespesasAllGroups(
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

            var query = GetDespesasFiltradas(
                queryDespesasAllGrupo,
                despesaFiltroDto.Filter,
                despesaFiltroDto.TipoFiltro
            );

            var listaPaginada = await Pagination.PaginateResultAsync(
                query.Select(d => d.MapToDTO()),
                despesaFiltroDto.PaginaAtual,
                despesaFiltroDto.ItensPorPagina
            );

            if (listaPaginada.Itens.IsNullOrEmpty())
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

        public async Task<List<DespesasSugestaoEconomiaQueryDto>> GetSugestoesEconomiaPorGrupoAsync()
        {
            var sugestoes = await _queryDespesasPorGrupo
                .Where(d =>
                    d.Categoria.Code != _categoriaIds.CodAluguel
                    && d.Categoria.Code != _categoriaIds.CodCondominio
                    && d.Categoria.Code != _categoriaIds.CodContaDeLuz
                    && d.Categoria.Code != _categoriaIds.CodInternet
                )
                .GroupBy(d => d.Item)
                .Where(g => g.Select(d => d.Fornecedor).Distinct().Count() > 1)
                .Select(group => new DespesasSugestaoEconomiaQueryDto
                {
                    Item = group.Key,
                    FornecedorMaisBarato = group.OrderBy(d => d.Preco).First().Fornecedor,
                    PrecoMaisBarato = group.Min(d => d.Preco),
                    PotencialEconomia = group.Max(d => d.Preco) - group.Min(d => d.Preco)
                })
                .Where(s => s.PotencialEconomia > 0)
                .ToListAsync();

            if (sugestoes.IsNullOrEmpty())
            {
                Notificar(EnumTipoNotificacao.Informacao, "Não há sugestões.");

                return [];
            }

            return sugestoes;
        }

        public async Task<IEnumerable<DespesasSugestaoDeFornecedorQueryDto>> SugestaoDeFornecedorMaisBarato(int paginaAtual, int itensPorPagina)
        {
            var queryDespesasPorGrupo = _repository
                .Get(d => d.GrupoFatura.Code == _grupoCode)
                .Include(c => c.Categoria)
                .Include(c => c.GrupoFatura);

            var categorias = await _categoriaRepository.Get().ToListAsync();

            List<DespesasSugestaoDeFornecedorQueryDto> sugestoes = new();

            foreach (var categoria in categorias)
            {
                var despesasSomenteCasa = _queryDespesasPorGrupo
                    .Where(d =>
                        d.Categoria.Code != _categoriaIds.CodAluguel
                        && d.Categoria.Code != _categoriaIds.CodCondominio
                        && d.Categoria.Code != _categoriaIds.CodContaDeLuz
                        && d.Categoria.Code != _categoriaIds.CodInternet
                    )
                    .Include(c => c.Categoria)
                    .Include(g => g.GrupoFatura)
                    .OrderByDescending(d => d.DataCompra);

                var itensAgrupados = await despesasSomenteCasa
                    .Where(d => d.Categoria.Code == categoria.Code)
                    .GroupBy(d => d.Item.ToLower())
                    .ToListAsync();

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

                case EnumFiltroDespesa.Preco:
                if (double.TryParse(filter, out _))
                {
                    string filterPreco = filter.Replace(",", ".");

                    query = query.Where(despesa =>
                        despesa.Preco.ToString().Contains(filterPreco)
                    );
                }
                break;
            }

            return query.OrderByDescending(d => d.DataCompra);
        }

        private async Task<PagedResult<DespesaQueryDto>> GetAllDespesas(
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

        #endregion
    }
}
