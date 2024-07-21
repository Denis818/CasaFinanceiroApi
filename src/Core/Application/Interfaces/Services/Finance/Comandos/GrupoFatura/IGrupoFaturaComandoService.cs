using Domain.Dtos.Despesas.Criacao;
using Domain.Enumeradores;
using Domain.Models.Despesas;

namespace Application.Interfaces.Services.Finance.Comandos
{
    public interface IGrupoFaturaComandoService
    {
        Task<bool> DeleteAsync(int id);
        Task<GrupoFatura> InsertAsync(GrupoFaturaDto grupoDto);
        Task<GrupoFatura> UpdateAsync(int id, GrupoFaturaDto grupoDto);
        Task<StatusFatura> UpdateStatusFaturaAsync(EnumFaturaTipo faturaNome, EnumStatusFatura status);
    }
}