using Domain.Dtos.QueryResults;

namespace Application.Queries.Interfaces
{
    public interface ICategoriaQueryServices
    {
        Task<IEnumerable<CategoriaQueryResult>> GetAllAsync();
        Task<CategoriaQueryResult> GetByCodigoAsync(Guid code);
    }
}