using Application.Configurations.MappingsApp;
using Application.Queries.Interfaces;
using Application.Queries.Services.Base;
using Domain.Dtos;
using Domain.Interfaces.Repositories;
using Domain.Models.Categorias;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Services
{
    public class CategoriaQueryServices(IServiceProvider service)
        : BaseQueryService<Categoria, CategoriaQueryDto, ICategoriaRepository>(service),
            ICategoriaQueryServices
    {
        protected override CategoriaQueryDto MapToDTO(Categoria entity) => entity.MapToDTO();

        public async Task<IEnumerable<CategoriaQueryDto>> GetAllAsync() =>
            await _repository
                .Get()
                .OrderBy(c => c.Descricao)
                .AsNoTracking()
                .Select(c => c.MapToDTO())
                .ToListAsync();

        public async Task<Categoria> GetByCodigoAsync(int id) => await GetByCodigoAsync(id);
    }
}
