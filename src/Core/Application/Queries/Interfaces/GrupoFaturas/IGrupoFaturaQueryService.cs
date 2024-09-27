using Application.Queries.Dtos;
using Domain.Dtos;

namespace Application.Queries.Interfaces
{
    public interface IGrupoFaturaQueryService
    {
        Task<IEnumerable<GrupoFaturaDto>> GetAllAsync(string ano);
        Task<string> GetNameFatura(Guid code);
        Task<StatusFaturaDto> GetStatusFaturaDtoByNameAsync(string status);
        Task<IEnumerable<GrupoFaturaSeletorQueryDto>> GetListGrupoFaturaParaSeletorAsync(string ano);
    }
}