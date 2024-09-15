using Domain.Dtos;
using Domain.Dtos.QueryResults;
using Domain.Interfaces.Repositories.Base;
using Domain.Models.Categorias;

namespace Domain.Interfaces.Repositories
{
    public interface ICategoriaRepository : IRepositoryBase<Categoria>
    {
        Task<Categoria> ExisteAsync(Guid? code = null, string nome = null);
        Task<bool> IdentificarCategoriaParaAcaoAsync(Guid codeCategoria);
        Task<CategoriaCodsDto> GetCategoriaCodesAsync();
        Task<IEnumerable<CategoriaQueryResult>> GetAll(Guid grupoCode);
    }
}
