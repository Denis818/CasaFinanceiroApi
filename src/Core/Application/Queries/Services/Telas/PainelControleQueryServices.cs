using Application.Configurations.MappingsApp;
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
    public class PainelControleQueryServices(
        IServiceProvider service,
        IDespesaFiltroService despesaFiltro,
        IParametroDeAlertaDeGastosRepository _parametroDeAlertaDeGastosRepository
    )
        : BaseQueryService<Despesa, DespesaDto, IDespesaRepository>(service),
            IPainelControleQueryServices
    {
        protected override DespesaDto MapToDTO(Despesa entity) => entity.MapToDTO();

        #region Painel de Controle

        public async Task<PagedResult<DespesaDto>> GetListDespesasPorGrupo(
            DespesaFiltroDto despesaFiltroDto
        )
        {
            if (string.IsNullOrEmpty(despesaFiltroDto.Filter))
            {
                return await GetAllDespesas(
                    QueryDespesasPorGrupo,
                    despesaFiltroDto.PaginaAtual,
                    despesaFiltroDto.ItensPorPagina
                );
            }

            var query = despesaFiltro.GetDespesasFiltradas(
                QueryDespesasPorGrupo,
                despesaFiltroDto.Filter,
                despesaFiltroDto.TipoFiltro
            );

            var listaPaginada = await Pagination.PaginateResultAsync(
                query.Select(d => d.MapToDTO()),
                despesaFiltroDto.PaginaAtual,
                despesaFiltroDto.ItensPorPagina
            );

            return listaPaginada;
        }

        public async Task<(double, double)> CompararFaturaComTotalDeDespesas(double faturaCartao)
        {
            double totalDespesas = await QueryDespesasPorGrupo
                .Where(despesa =>
                    despesa.Categoria.Code != _categoriaIds.CodAluguel
                    && despesa.Categoria.Code != _categoriaIds.CodContaDeLuz
                    && despesa.Categoria.Code != _categoriaIds.CodCondominio
                )
                .SumAsync(despesa => despesa.Total);

            double valorSubtraido = totalDespesas - faturaCartao;

            return (totalDespesas, valorSubtraido);
        }

        public async Task<IList<ParametroDeAlertaDeGastos>> GetParametroDeAlertaDeGastos()
        {
            return await _parametroDeAlertaDeGastosRepository.Get().ToListAsync();
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
    }
}
