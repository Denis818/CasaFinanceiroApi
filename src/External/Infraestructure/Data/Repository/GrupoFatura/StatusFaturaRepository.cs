using Data.DataContext;
using Data.Repository.Base;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;

namespace Infraestructure.Data.Repository
{
    public class StatusFaturaRepository(IServiceProvider service) :
        RepositoryBase<StatusFatura, FinanceDbContext>(service), IStatusFaturaRepository
    {
    }
}
