using Application.Commands.Dtos;
using Domain.Models.Categorias;

namespace Application.Commands.Interfaces
{
    public interface ICategoriaCommandServices
    {
        Task<bool> DeleteAsync(int id);
        Task<Categoria> InsertAsync(CategoriaCommandDto categoriaDto);
        Task<Categoria> UpdateAsync(int id, CategoriaCommandDto categoriaDto);
    }
}