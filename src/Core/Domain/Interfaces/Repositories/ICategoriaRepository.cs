using Domain.Dtos;
using Domain.Interfaces.Repositories.Base;
using Domain.Models.Categorias;

namespace Domain.Interfaces.Repositories
{
    public interface ICategoriaRepository : IRepositoryBase<Categoria>
    {
        Task<Categoria> ExisteAsync(int id = 0, string nome = null);
        bool IdentificarCategoriaParaAcao(int idCategoria);
        CategoriaIdsDto GetCategoriaIds();
    }
}
