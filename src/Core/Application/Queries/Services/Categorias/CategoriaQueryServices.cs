using Application.Configurations.MappingsApp;
using Application.Queries.Interfaces;
using Application.Queries.Services.Base;
using Domain.Dtos.Categoria;
using Domain.Interfaces.Repositories.Categorias;
using Domain.Models.Categorias;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Services
{
    public class CategoriaQueryServices(IServiceProvider service)
        : BaseQueryService<Categoria, CategoriaDto, ICategoriaRepository>(service),
            ICategoriaQueryServices
    {
        protected override CategoriaDto MapToDTO(Categoria entity) => entity.MapToDTO();

        public async Task<IEnumerable<CategoriaDto>> GetAllAsync()
        {
            var resultado = await _repository.Get()
                .Select(c => new CategoriaDto
                {
                    Code = c.Code,
                    Descricao = c.Descricao,
                    Total = c.Despesas
                        .Where(d => d.GrupoFaturaCode == _grupoCode)
                        .Sum(d => d.Total),
                    QuantidadeDeItens = c.Despesas
                        .Where(d => d.GrupoFaturaCode == _grupoCode)
                        .Count()
                })
                .OrderBy(c => c.Descricao)
                .ToListAsync();

            return resultado;
        }

        public async Task<Categoria> GetByCodigoAsync(int id) => await GetByCodigoAsync(id);
    }
}
