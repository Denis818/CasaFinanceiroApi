using Application.Queries.Dtos;
using Domain.Models.Despesas;
using Domain.Utilities;

namespace Application.Queries.Interfaces
{
    public interface IPainelControleQueryServices
    {
        Task<(double, double)> CompararFaturaComTotalDeDespesas(double faturaCartao);
        Task<PagedResult<Despesa>> GetListDespesasPorGrupo(DespesaFiltroDto despesaFiltroDto);
    }
}