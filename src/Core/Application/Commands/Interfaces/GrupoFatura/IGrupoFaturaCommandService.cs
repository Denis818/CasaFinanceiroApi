using Application.Commands.Dtos;
using Domain.Enumeradores;
using Domain.Models.Despesas;

namespace Application.Commands.Interfaces
{
    public interface IGrupoFaturaCommandService
    {
        Task<bool> DeleteAsync(int id);
        Task<GrupoFatura> InsertAsync(GrupoFaturaCommandDto grupoDto);
        Task<GrupoFatura> UpdateAsync(int id, GrupoFaturaCommandDto grupoDto);
        Task<StatusFatura> UpdateStatusFaturaAsync(EnumFaturaTipo faturaNome, EnumStatusFatura status);
    }
}