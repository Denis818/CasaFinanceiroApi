using Data.DataContext;
using Data.Repository.Base;
using Domain.Interfaces.Repositories.Cobrancas;
using Domain.Models.Cobrancas;

namespace Infraestructure.Data.Repository.Cobrancas
{
    public class PagamentoRepository(IServiceProvider service)
        : RepositoryBase<Pagamento, FinanceDbContext>(service), IPagamentoRepository

    {
    }
}
