using Application.Queries.Interfaces;
using Application.Queries.Services.Base;
using Domain.Interfaces.Repositories;
using Domain.Models.Categorias;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Services
{
    public class CategoriaQueryServices(IServiceProvider service)
        : BaseQueryService<Categoria, ICategoriaRepository>(service), ICategoriaQueryServices
    {
        public async Task<IEnumerable<Categoria>> GetAllAsync() =>
            await _repository.Get().AsNoTracking().OrderBy(c => c.Descricao).ToListAsync();

        public async Task<Categoria> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);
    }
}
