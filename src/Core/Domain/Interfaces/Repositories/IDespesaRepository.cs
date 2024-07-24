using Domain.Dtos;
using Domain.Dtos.QueryResults.Despesas;
using Domain.Interfaces.Repositories.Base;
using Domain.Models.Despesas;

namespace Domain.Interfaces.Repositories
{
    public interface IDespesaRepository : IRepositoryBase<Despesa>
    {
        Task<RelatorioGastosDoGrupoResult> GetRelatorioDeGastosDoGrupoAsync(
            int grupoId,
            CategoriaIdsDto categoriaIds
        );

        Task<IEnumerable<DespesasPorGrupoResult>> GetDespesaGrupoParaGraficoAsync(string ano);
        Task<IEnumerable<TotalPorCategoriaQueryResut>> GetTotalPorCategoriaAsync(int grupoId);
    }
}
