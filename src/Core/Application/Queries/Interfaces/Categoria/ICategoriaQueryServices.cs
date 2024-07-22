using Domain.Models.Categorias;

namespace Application.Queries.Interfaces
{
    public interface ICategoriaQueryServices
    {
        Task<IEnumerable<Categoria>> GetAllAsync();
        Task<Categoria> GetByIdAsync(int id);
    }
}