using Data.DataContext;
using Data.Repository.Base;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Data.Repository
{
    public class GrupoFaturaRepository(IServiceProvider service)
        : RepositoryBase<GrupoFatura, FinanceDbContext>(service),
            IGrupoFaturaRepository
    {
        public async Task<GrupoFatura> ExisteAsync(int id, string nome)
        {
            GrupoFatura GrupoFatura = null;
            if (nome != null)
            {
                GrupoFatura = await Get(c => c.Nome == nome).FirstOrDefaultAsync();
            }
            else
            {
                GrupoFatura = await Get(c => c.Id == id).FirstOrDefaultAsync();
            }

            return GrupoFatura;
        }
    }
}
