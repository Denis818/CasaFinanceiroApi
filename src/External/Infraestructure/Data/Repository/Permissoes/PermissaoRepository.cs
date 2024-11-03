using Data.DataContext;
using Data.Repository.Base;
using Domain.Interfaces.Repositories.Permissoes;
using Domain.Models.Users;

namespace Infraestructure.Data.Repository.Permissoes
{
    public class PermissaoRepository(IServiceProvider service)
     : RepositoryBase<Permissao, FinanceDbContext>(service),
         IPermissaoRepository
    { }
}
