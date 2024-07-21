using Domain.Models.Membros;

namespace Application.Interfaces.Services.Finance.Consultas
{
    public interface IMembroConsultaServices
    {
        Task<string> EnviarValoresDividosPeloWhatsAppAsync(string nome, string titleMessage, bool isMoradia, string pix);
        Task<IEnumerable<Membro>> GetAllAsync();
        Task<Membro> GetByIdAsync(int id);
    }
}