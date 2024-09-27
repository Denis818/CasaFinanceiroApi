using Data.DataContext;
using Data.Repository.Base;
using Domain.Interfaces.Repositories.ListaCompras;
using Domain.Models.ListaCompras;

namespace Infraestructure.Data.Repository.ListaCompras
{
    public class ProdutoListaComprasRepository(IServiceProvider service)
        : RepositoryBase<ProdutoListaCompras, FinanceDbContext>(service), IProdutoListaComprasRepository
    { }
}
