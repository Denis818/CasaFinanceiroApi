using Data.DataContext;
using Data.Repository.Base;
using Domain.Interfaces.Repositories.GrupoFaturas;
using Domain.Models.GrupoFaturas;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Data.Repository
{
    public class GrupoFaturaRepository(IServiceProvider service)
        : RepositoryBase<GrupoFatura, FinanceDbContext>(service),
            IGrupoFaturaRepository
    {
        public async Task<GrupoFatura> ExisteAsync(Guid? code, string nome)
        {
            GrupoFatura GrupoFatura = null;
            if (nome != null)
            {
                GrupoFatura = await Get(c => c.Nome == nome).FirstOrDefaultAsync();
            }
            else
            {
                GrupoFatura = await Get(c => c.Code == code).FirstOrDefaultAsync();
            }

            return GrupoFatura;
        }


        public Lazy<GrupoFatura> GetGrupoFatura(Guid grupoCode)
        {
            return new Lazy<GrupoFatura>(() =>
            {
                return Get(g => g.Code == grupoCode).FirstOrDefault();
            });
        }
    }
}
