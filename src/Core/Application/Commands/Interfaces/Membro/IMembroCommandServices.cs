using Application.Commands.Dtos;

namespace Application.Commands.Interfaces
{
    public interface IMembroComandoServices
    {
        Task<bool> DeleteAsync(Guid code);
        Task InsertAsync(MembroCommandDto membroDto);
        Task UpdateAsync(Guid code, MembroCommandDto membroDto);
    }
}