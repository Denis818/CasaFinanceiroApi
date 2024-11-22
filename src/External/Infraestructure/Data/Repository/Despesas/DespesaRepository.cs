using Data.DataContext;
using Data.Repository.Base;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository.Despesas
{
    public class DespesaRepository(IServiceProvider service)
        : RepositoryBase<Despesa, FinanceDbContext>(service),
            IDespesaRepository
    {
        public Lazy<IList<Despesa>> GetListDespesasPorGrupo(Guid grupoCod)
        {
            return new Lazy<IList<Despesa>>(
                () =>
                    Get(d => d.GrupoFaturaCode == grupoCod)
                        .Include(c => c.Categoria)
                        .Include(g => g.GrupoFatura)
                        .Include(g => g.GrupoFatura.StatusFaturas)
                        .ToListAsync()
                        .Result
            );
        }
    }
}
