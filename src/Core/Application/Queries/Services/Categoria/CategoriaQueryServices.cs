using Application.Configurations.MappingsApp;
using Application.Queries.Interfaces;
using Application.Queries.Services.Base;
using Domain.Dtos.QueryResults;
using Domain.Interfaces.Repositories;
using Domain.Models.Categorias;

namespace Application.Queries.Services
{
    public class CategoriaQueryServices(IServiceProvider service)
        : BaseQueryService<Categoria, CategoriaQueryResult, ICategoriaRepository>(service),
            ICategoriaQueryServices
    {
        protected override CategoriaQueryResult MapToDTO(Categoria entity) => entity.MapToDTO();

        public async Task<IEnumerable<CategoriaQueryResult>> GetAllAsync() =>
            await _repository.GetAll(_grupoCode);

        public async Task<Categoria> GetByCodigoAsync(int id) => await GetByCodigoAsync(id);
    }
}
