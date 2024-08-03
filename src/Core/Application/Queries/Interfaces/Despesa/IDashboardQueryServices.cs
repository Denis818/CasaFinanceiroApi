using Application.Queries.Dtos;
using Domain.Dtos;
using Domain.Dtos.QueryResults.Despesas;

namespace Application.Queries.Interfaces
{
    public interface IDashboardQueryServices
    {
        Task<byte[]> DownloadPdfRelatorioDeDespesaCasa();
        Task<byte[]> DownloadPdfRelatorioDeDespesaMoradia();
        Task<DespesasDivididasMensalQueryDto> GetDespesasDivididasMensalAsync();
        Task<IEnumerable<DespesasPorGrupoQueryResult>> GetDespesaGrupoParaGraficoAsync(string ano);
        Task<IEnumerable<TotalPorCategoriaQueryResult>> GetTotalPorCategoriaAsync();
        Task<RelatorioGastosDoGrupoQueryResult> GetRelatorioDeGastosDoGrupoAsync();
    }
}