using Domain.Interfaces.Repositories.Base;
using Domain.Models.GrupoFaturas;

namespace Domain.Interfaces.Repositories.GrupoFaturas
{
    public interface IGrupoFaturaRepository : IRepositoryBase<GrupoFatura>
    {
        Task<GrupoFatura> ExisteAsync(Guid? code = null, string nome = null);

        Lazy<GrupoFatura> GetGrupoFatura(Guid grupoCode);
    }
}
