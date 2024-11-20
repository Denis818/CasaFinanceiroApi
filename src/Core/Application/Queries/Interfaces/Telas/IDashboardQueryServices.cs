using Application.Queries.Dtos;

namespace Application.Queries.Interfaces.Telas
{
    public interface IDashboardQueryServices
    {
        byte[] ExportarPdfRelatorioDeDespesaCasa();
        byte[] ExportarPdfRelatorioDeDespesaMoradia();
        DespesasDivididasMensalQueryDto GetDespesasDivididasMensal();
        Task<IEnumerable<DespesasPorGrupoQueryDto>> GetDespesaGrupoParaGraficoAsync(string ano);
        Task<RelatorioGastosDoGrupoQueryDto> GetRelatorioDeGastosDoGrupoAsync();
    }
}
