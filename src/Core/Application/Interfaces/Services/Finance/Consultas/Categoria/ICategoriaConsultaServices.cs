using Domain.Models.Categorias;

namespace Application.Interfaces.Services.Finance.Consultas
{
    public interface ICategoriaConsultaServices
    {
        Task<IEnumerable<Categoria>> GetAllAsync();
        Task<Categoria> GetByIdAsync(int id);
    }
}