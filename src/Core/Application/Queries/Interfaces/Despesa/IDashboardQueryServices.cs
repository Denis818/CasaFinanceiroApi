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
        Task<IEnumerable<DespesasPorGrupoResult>> GetDespesaGrupoParaGraficoAsync(string ano);
        Task<IEnumerable<TotalPorCategoriaQueryResut>> GetTotalPorCategoriaAsync();
    }
}