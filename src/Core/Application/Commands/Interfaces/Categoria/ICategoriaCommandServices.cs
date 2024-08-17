using Application.Commands.Dtos;

namespace Application.Commands.Interfaces
{
    public interface ICategoriaCommandServices
    {
        Task<bool> DeleteAsync(Guid code);
        Task InsertAsync(CategoriaCommandDto categoriaDto);
        Task UpdateAsync(Guid code, CategoriaCommandDto categoriaDto);
    }
}