using Domain.Dtos;

namespace Application.Queries.Interfaces
{
    public interface ICategoriaQueryServices
    {
        Task<IEnumerable<CategoriaQueryDto>> GetAllAsync();
        Task<CategoriaQueryDto> GetByCodigoAsync(Guid code);
    }
}