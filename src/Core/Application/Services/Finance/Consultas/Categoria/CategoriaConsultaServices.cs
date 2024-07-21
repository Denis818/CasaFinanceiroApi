using Application.Interfaces.Services.Finance.Consultas;
using Application.Services.Base;
using Domain.Interfaces.Repositories;
using Domain.Models.Categorias;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Finance.Consultas
{
    public class CategoriaConsultaServices(IServiceProvider service)
        : BaseAppService<Categoria, ICategoriaRepository>(service), ICategoriaConsultaServices
    {
        public async Task<IEnumerable<Categoria>> GetAllAsync() =>
            await _repository.Get().OrderBy(c => c.Descricao).ToListAsync();

        public async Task<Categoria> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);
    }
}
