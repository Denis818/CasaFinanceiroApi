using Application.Queries.Dtos;

namespace Application.Queries.Interfaces.Telas
{
    public interface IDashboardQueryServices
    {
        Task<byte[]> ExportarPdfRelatorioDeDespesaCasa();
        Task<byte[]> ExportarPdfRelatorioDeDespesaMoradia();
        Task<DespesasDivididasMensalQueryDto> GetDespesasDivididasMensal();
        Task<IEnumerable<DespesasPorGrupoQueryDto>> GetDespesaGrupoParaGraficoAsync(string ano);
        Task<RelatorioGastosDoGrupoQueryDto> GetRelatorioDeGastosDoGrupoAsync();
    }
}
