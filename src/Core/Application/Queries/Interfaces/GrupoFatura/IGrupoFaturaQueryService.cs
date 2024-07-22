using Application.Queries.Dtos;

namespace Application.Queries.Interfaces
{
    public interface IGrupoFaturaQueryService
    {
        Task<IEnumerable<GrupoFaturaQueryDto>> GetAllAsync(string ano);
        Task<string> GetNameFatura(int id);
        Task<StatusFaturaQueryDto> GetStatusFaturaDtoByNameAsync(string status);
        Task<IEnumerable<GrupoFaturaSeletorQueryDto>> GetListGrupoFaturaParaSeletorAsync(string ano);
    }
}