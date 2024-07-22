using Domain.Models.Membros;

namespace Application.Queries.Interfaces
{
    public interface IMembroQueryServices
    {
        Task<string> EnviarValoresDividosPeloWhatsAppAsync(string nome, string titleMessage, bool isMoradia, string pix);
        Task<IEnumerable<Membro>> GetAllAsync();
        Task<Membro> GetByIdAsync(int id);
    }
}