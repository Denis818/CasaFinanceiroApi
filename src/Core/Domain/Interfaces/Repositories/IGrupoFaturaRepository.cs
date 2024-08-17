using Domain.Interfaces.Repositories.Base;
using Domain.Models.Despesas;

namespace Domain.Interfaces.Repositories
{
    public interface IGrupoFaturaRepository : IRepositoryBase<GrupoFatura>
    {
        Task<GrupoFatura> ExisteAsync(Guid? code = null, string nome = null);
    }
}
