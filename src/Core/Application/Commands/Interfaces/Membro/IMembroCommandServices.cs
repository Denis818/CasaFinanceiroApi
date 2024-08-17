using Application.Commands.Dtos;

namespace Application.Commands.Interfaces
{
    public interface IMembroComandoServices
    {
        Task<bool> DeleteAsync(Guid code);
        Task<bool> InsertAsync(MembroCommandDto membroDto);
        Task<bool> UpdateAsync(Guid code, MembroCommandDto membroDto);
    }
}