using Data.DataContext;
using Data.Repository.Base;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;

namespace Infraestructure.Data.Repository.Despesas
{
    public class ProdutoListaComprasRepository(IServiceProvider service)
        : RepositoryBase<ProdutoListaCompras, FinanceDbContext>(service), IProdutoListaComprasRepository
    { }
}
