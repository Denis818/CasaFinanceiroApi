using Application.Queries.Dtos;
using Domain.Dtos.QueryResults.Despesas;

namespace Application.Queries.Interfaces
{
    public interface IDashboardQueryServices
    {
        Task<byte[]> ExportarPdfRelatorioDeDespesaCasa();
        Task<byte[]> ExportarPdfRelatorioDeDespesaMoradia();
        Task<DespesasDivididasMensalQueryDto> GetDespesasDivididasMensalAsync();
        Task<IEnumerable<DespesasPorGrupoQueryResult>> GetDespesaGrupoParaGraficoAsync(string ano);
        Task<RelatorioGastosDoGrupoQueryResult> GetRelatorioDeGastosDoGrupoAsync();
    }
}