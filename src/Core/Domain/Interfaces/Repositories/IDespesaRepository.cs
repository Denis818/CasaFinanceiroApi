using Domain.Dtos;
using Domain.Dtos.QueryResults.Despesas;
using Domain.Interfaces.Repositories.Base;
using Domain.Models.Despesas;

namespace Domain.Interfaces.Repositories
{
    public interface IDespesaRepository : IRepositoryBase<Despesa>
    {
        Task<RelatorioGastosDoGrupoQueryResult> GetRelatorioDeGastosDoGrupoAsync(
            int grupoId,
            CategoriaIdsDto categoriaIds
        );

        Task<IEnumerable<DespesasPorGrupoQueryResult>> GetDespesaGrupoParaGraficoAsync(string ano);
        Task<IEnumerable<TotalPorCategoriaQueryResult>> GetTotalPorCategoriaAsync(int grupoId);
    }
}
