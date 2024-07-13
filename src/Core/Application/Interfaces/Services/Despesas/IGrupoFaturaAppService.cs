using Domain.Dtos.Despesas.Criacao;
using Domain.Models.Despesas;

namespace Application.Interfaces.Services.Despesas
{
    public interface IGrupoFaturaAppService
    {
        Task<IEnumerable<GrupoFatura>> GetAllAsync();
        Task<string> GetNameFatura(int id);
        Task<GrupoFatura> InsertAsync(GrupoFaturaDto grupoDto);
        Task<GrupoFatura> UpdateAsync(int id, GrupoFaturaDto GrupoFaturaDto);
        Task<bool> DeleteAsync(int id);
    }
}