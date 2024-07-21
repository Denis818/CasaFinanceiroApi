using Domain.Dtos.Despesas.Consultas;

namespace Application.Interfaces.Services.Finance.Consultas
{
    public interface IDashboardConsultaServices
    {
        Task<byte[]> DownloadPdfRelatorioDeDespesaCasa();
        Task<byte[]> DownloadPdfRelatorioDeDespesaMoradia();
        Task<DespesasDivididasMensalDto> GetAnaliseDesesasPorGrupoAsync();
        Task<IEnumerable<DespesasPorGrupoDto>> GetDespesaGrupoParaGraficoAsync();
        Task<IEnumerable<DespesasTotalPorCategoriaDto>> GetTotalPorCategoriaAsync();
    }
}