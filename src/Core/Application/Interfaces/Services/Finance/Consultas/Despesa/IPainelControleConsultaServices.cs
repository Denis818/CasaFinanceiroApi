using Domain.Dtos.Despesas.Filtro;
using Domain.Enumeradores;
using Domain.Models.Despesas;
using Domain.Utilities;

namespace Application.Interfaces.Services.Finance.Consultas
{
    public interface IPainelControleConsultaServices
    {
        Task<(double, double)> CompararFaturaComTotalDeDespesas(double faturaCartao);
        Task<PagedResult<Despesa>> GetListDespesasPorGrupo(DespesaFiltroDto despesaFiltroDto);
    }
}