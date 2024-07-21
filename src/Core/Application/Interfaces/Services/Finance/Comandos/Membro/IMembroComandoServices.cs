using Domain.Dtos.Membros;
using Domain.Models.Membros;

namespace Application.Interfaces.Services.Finance.Comandos
{
    public interface IMembroComandoServices
    {
        Task<bool> DeleteAsync(int id);
        Task<Membro> InsertAsync(MembroDto membroDto);
        Task<Membro> UpdateAsync(int id, MembroDto membroDto);
    }
}