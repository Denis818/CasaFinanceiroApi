using Domain.Dtos;
using Domain.Interfaces.Repositories.Base;
using Domain.Models.Categorias;

namespace Domain.Interfaces.Repositories.Categorias
{
    public interface ICategoriaRepository : IRepositoryBase<Categoria>
    {
        Task<Categoria> ExisteAsync(Guid? code = null, string nome = null);
        Lazy<CategoriaCodsDto> GetCategoriaCods();
    }
}
