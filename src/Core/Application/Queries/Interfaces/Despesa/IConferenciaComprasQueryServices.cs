using Application.Queries.Dtos;
using Domain.Models.Despesas;
using Domain.Utilities;

namespace Application.Queries.Interfaces
{
    public interface IConferenciaComprasQueryServices
    {
        Task<PagedResult<Despesa>> GetListDespesasAllGroups(DespesaFiltroDto despesaFiltroDto, string ano);
        Task<List<DespesasSugestaoEconomiaQueryDto>> GetSugestoesEconomiaPorGrupoAsync();
        Task<IEnumerable<DespesasSugestaoDeFornecedorQueryDto>> SugestaoDeFornecedorMaisBarato(int paginaAtual, int itensPorPagina);
    }
}