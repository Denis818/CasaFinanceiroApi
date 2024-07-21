using Application.Interfaces.Services.Finance.Consultas;
using Application.Resources.Messages;
using Application.Services.Base;
using Application.Utilities;
using Domain.Dtos.Categorias.Consultas;
using Domain.Dtos.Despesas.Filtro;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Domain.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Finance.Consultas
{
    public class PainelControleConsultaServices
        : BaseAppService<Despesa, IDespesaRepository>,
            IPainelControleConsultaServices
    {
        private readonly CategoriaIdsDto _categoriaIds;
        private readonly IQueryable<Despesa> _queryDespesasPorGrupo;

        public PainelControleConsultaServices(
            IServiceProvider service,
            ICategoriaRepository categoriaRepository
        )
            : base(service)
        {
            _categoriaIds = categoriaRepository.GetCategoriaIds();

            int grupoId = (int)(_httpContext.Items["GrupoFaturaId"] ?? 0);

            _queryDespesasPorGrupo = _repository
                .Get(d => d.GrupoFaturaId == grupoId)
                .Include(c => c.Categoria)
                .Include(c => c.GrupoFatura);
        }

        #region Painel de Controle

        public async Task<PagedResult<Despesa>> GetListDespesasPorGrupo(
            DespesaFiltroDto despesaFiltroDto
        )
        {
            if (string.IsNullOrEmpty(despesaFiltroDto.Filter))
            {
                return await GetAllDespesas(
                    _queryDespesasPorGrupo,
                    despesaFiltroDto.PaginaAtual,
                    despesaFiltroDto.ItensPorPagina
                );
            }

            IOrderedQueryable<Despesa> query = GetDespesasFiltradas(
                _queryDespesasPorGrupo,
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

        public async Task<(double, double)> CompararFaturaComTotalDeDespesas(double faturaCartao)
        {
            double totalDespesas = await _queryDespesasPorGrupo
                .Where(despesa =>
                    despesa.CategoriaId != _categoriaIds.IdAluguel
                    && despesa.CategoriaId != _categoriaIds.IdContaDeLuz
                    && despesa.CategoriaId != _categoriaIds.IdCondominio
                )
                .SumAsync(despesa => despesa.Total);

            double valorSubtraido = totalDespesas - faturaCartao;

            return (totalDespesas, valorSubtraido);
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
