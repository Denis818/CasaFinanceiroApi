using Domain.Dtos.Despesas.Consultas;
using Domain.Dtos.Despesas.Filtro;
using Domain.Enumeradores;
using Domain.Models.Despesas;
using Domain.Utilities;

namespace Application.Interfaces.Services.Finance.Consultas
{
    public interface IConferenciaComprasConsultaServices
    {
        Task<PagedResult<Despesa>> GetListDespesasAllGroups(DespesaFiltroDto despesaFiltroDto, string ano);
        Task<List<SugestaoEconomiaInfoDto>> GetSugestoesEconomiaPorGrupoAsync();
        Task<IEnumerable<SugestaoDeFornecedorDto>> SugestaoDeFornecedorMaisBarato(int paginaAtual, int itensPorPagina);
    }
}