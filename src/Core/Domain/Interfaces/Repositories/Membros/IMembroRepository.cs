using Domain.Interfaces.Repositories.Base;
using Domain.Models.Membros;

namespace Domain.Interfaces.Repositories.Membros
{
    public interface IMembroRepository : IRepositoryBase<Membro>
    {
        Task<Membro> ExisteAsync(string nome);
        Task<bool> ValidaMembroParaAcao(Guid codeMembro);
    }
}
