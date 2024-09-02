using Application.Queries.Dtos;
using Domain.Dtos;
using Domain.Utilities;

namespace Application.Queries.Interfaces
{
    public interface IAuditoriaComprasQueryServices
    {
        Task<PagedResult<DespesaQueryDto>> GetListDespesasAllGroups(DespesaFiltroDto despesaFiltroDto, string ano);
        Task<List<DespesasSugestaoEconomiaQueryDto>> GetSugestoesEconomiaPorGrupoAsync();
        Task<IEnumerable<DespesasSugestaoDeFornecedorQueryDto>> SugestaoDeFornecedorMaisBarato(int paginaAtual, int itensPorPagina);
    }
}