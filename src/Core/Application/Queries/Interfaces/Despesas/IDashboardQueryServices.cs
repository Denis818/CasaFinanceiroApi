using Application.Queries.Dtos;

namespace Application.Queries.Interfaces
{
    public interface IDashboardQueryServices
    {
        Task<byte[]> ExportarPdfRelatorioDeDespesaCasa();
        Task<byte[]> ExportarPdfRelatorioDeDespesaMoradia();
        Task<DespesasDivididasMensalQueryDto> GetDespesasDivididasMensalAsync();
        Task<IEnumerable<DespesasPorGrupoQueryDto>> GetDespesaGrupoParaGraficoAsync(string ano);
        Task<RelatorioGastosDoGrupoQueryDto> GetRelatorioDeGastosDoGrupoAsync();
    }
}