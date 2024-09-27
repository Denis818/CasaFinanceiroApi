using Domain.Dtos;

namespace Application.Queries.Interfaces
{
    public interface IMembroQueryServices
    {
        Task<string> EnviarValoresDividosPeloWhatsAppAsync(string nome, string titleMessage, bool isMoradia, string pix);
        Task<IEnumerable<MembroDto>> GetAllAsync();
        Task<MembroDto> GetByCodigoAsync(Guid code);
    }
}