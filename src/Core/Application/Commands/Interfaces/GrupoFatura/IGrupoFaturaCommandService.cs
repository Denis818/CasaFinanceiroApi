using Application.Commands.Dtos;
using Domain.Enumeradores;

namespace Application.Commands.Interfaces
{
    public interface IGrupoFaturaCommandService
    {
        Task<bool> DeleteAsync(Guid code);
        Task<bool> InsertAsync(GrupoFaturaCommandDto grupoDto);
        Task<bool> UpdateAsync(Guid code, GrupoFaturaCommandDto grupoDto);
        Task<bool> UpdateStatusFaturaAsync(EnumFaturaTipo faturaNome, EnumStatusFatura status);
    }
}