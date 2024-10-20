using Data.DataContext;
using Data.Repository.Base;
using Domain.Interfaces.Repositories.Compras;
using Domain.Models.Compras;

namespace Infraestructure.Data.Repository.Compras
{
    public class CompraRepository(IServiceProvider service)
        : RepositoryBase<Compra, FinanceDbContext>(service),
            ICompraRepository
    { }
}
