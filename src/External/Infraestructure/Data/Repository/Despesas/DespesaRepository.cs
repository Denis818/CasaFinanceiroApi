using Data.DataContext;
using Data.Repository.Base;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;

namespace Data.Repository.Despesas
{
    public class DespesaRepository(IServiceProvider service)
        : RepositoryBase<Despesa, FinanceDbContext>(service),
            IDespesaRepository
    {
    }
}
