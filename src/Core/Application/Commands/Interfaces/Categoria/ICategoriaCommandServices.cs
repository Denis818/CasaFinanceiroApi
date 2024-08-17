using Application.Commands.Dtos;

namespace Application.Commands.Interfaces
{
    public interface ICategoriaCommandServices
    {
        Task<bool> DeleteAsync(Guid code);
        Task<bool> InsertAsync(CategoriaCommandDto categoriaDto);
        Task<bool> UpdateAsync(Guid code, CategoriaCommandDto categoriaDto);
    }
}