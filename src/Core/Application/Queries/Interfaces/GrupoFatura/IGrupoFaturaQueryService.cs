using Application.Queries.Dtos;
using Domain.Models.Despesas;

namespace Application.Queries.Interfaces
{
    public interface IGrupoFaturaQueryService
    {
        Task<IEnumerable<GrupoFatura>> GetAllAsync(string ano);
        Task<string> GetNameFatura(int id);
        Task<StatusFaturaQueryDto> GetStatusFaturaDtoByNameAsync(string status);
    }
}