using Application.Commands.Dtos;
using Domain.Enumeradores;

namespace Application.Commands.Interfaces
{
    public interface IGrupoFaturaCommandService
    {
        Task<bool> DeleteAsync(Guid code);
        Task InsertAsync(GrupoFaturaCommandDto grupoDto);
        Task UpdateAsync(Guid code, GrupoFaturaCommandDto grupoDto);
        Task UpdateStatusFaturaAsync(EnumFaturaTipo faturaNome, EnumStatusFatura status);
    }
}