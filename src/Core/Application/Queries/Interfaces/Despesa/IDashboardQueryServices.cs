using Application.Queries.Dtos;

namespace Application.Queries.Interfaces
{
    public interface IDashboardQueryServices
    {
        Task<byte[]> DownloadPdfRelatorioDeDespesaCasa();
        Task<byte[]> DownloadPdfRelatorioDeDespesaMoradia();
        Task<DespesasDivididasMensalQueryDto> GetAnaliseDesesasPorGrupoAsync();
        Task<IEnumerable<DespesasPorGrupoQueryDto>> GetDespesaGrupoParaGraficoAsync(string ano);
        Task<IEnumerable<DespesasTotalPorCategoriaQueryDto>> GetTotalPorCategoriaAsync();
    }
}