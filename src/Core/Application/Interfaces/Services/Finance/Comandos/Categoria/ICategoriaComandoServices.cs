using Domain.Dtos.Categorias;
using Domain.Models.Categorias;

namespace Application.Interfaces.Services.Finance.Comandos
{
    public interface ICategoriaComandoServices
    {
        Task<bool> DeleteAsync(int id);
        Task<Categoria> InsertAsync(CategoriaDto categoriaDto);
        Task<Categoria> UpdateAsync(int id, CategoriaDto categoriaDto);
    }
}