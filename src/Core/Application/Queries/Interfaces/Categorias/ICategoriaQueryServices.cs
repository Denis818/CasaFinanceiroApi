using Domain.Dtos.Categoria;

namespace Application.Queries.Interfaces
{
    public interface ICategoriaQueryServices
    {
        Task<IEnumerable<CategoriaDto>> GetAllAsync();
        Task<CategoriaDto> GetByCodigoAsync(Guid code);
    }
}