using Application.Queries.Dtos;
using Domain.Dtos.QueryResults;

namespace Application.Queries.Interfaces
{
    public interface IGrupoFaturaQueryService
    {
        Task<IEnumerable<GrupoFaturaQueryResult>> GetAllAsync(string ano);
        Task<string> GetNameFatura(int id);
        Task<StatusFaturaQueryDto> GetStatusFaturaDtoByNameAsync(string status);
        Task<IEnumerable<GrupoFaturaSeletorQueryDto>> GetListGrupoFaturaParaSeletorAsync(string ano);
    }
}