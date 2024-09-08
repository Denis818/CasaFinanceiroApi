using Application.Queries.Dtos;
using Domain.Dtos;
using Domain.Models.Despesas;
using Domain.Utilities;

namespace Application.Queries.Interfaces
{
    public interface IPainelControleQueryServices
    {
        Task<(double, double)> CompararFaturaComTotalDeDespesas(double faturaCartao);
        Task<PagedResult<DespesaQueryDto>> GetListDespesasPorGrupo(DespesaFiltroDto despesaFiltroDto);
        Task<IList<ParametroDeAlertaDeGastos>> GetParametroDeAlertaDeGastos();
        Task<byte[]> ExportaPdfListaDeComprasAsync();
    }
}