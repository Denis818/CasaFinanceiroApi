using Application.Commands.Dtos;
using Domain.Models.Membros;

namespace Application.Commands.Interfaces
{
    public interface IMembroComandoServices
    {
        Task<bool> DeleteAsync(int id);
        Task<Membro> InsertAsync(MembroCommandDto membroDto);
        Task<Membro> UpdateAsync(int id, MembroCommandDto membroDto);
    }
}