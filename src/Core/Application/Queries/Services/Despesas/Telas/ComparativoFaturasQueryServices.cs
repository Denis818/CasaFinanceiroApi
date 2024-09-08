using Application.Configurations.MappingsApp;
using Application.Queries.Dtos;
using Application.Queries.Interfaces;
using Application.Queries.Services.Base;
using Domain.Dtos;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Services.Telas
{
    public class ComparativoFaturasQueryServices(IServiceProvider service)
        : BaseQueryService<Despesa, DespesaQueryDto, IDespesaRepository>(service),
            IComparativoFaturasQueryServices
    {
        protected override DespesaQueryDto MapToDTO(Despesa entity) => entity.MapToDTO();

        public async Task<List<ComparativoFaturasQueryDto>> GetComparativoFaturasAsync(
            Guid grupoFaturaCode1,
            Guid grupoFaturaCode2
        )
        {
            var categorias = await _categoriaRepository.Get().ToListAsync();

            var despesas = await _repository
                .Get()
                .Include(c => c.Categoria)
                .Where(d =>
                    d.GrupoFaturaCode == grupoFaturaCode1 || d.GrupoFaturaCode == grupoFaturaCode2
                )
                .ToListAsync();

            var resultado = categorias
                .Select(categoria => new ComparativoFaturasQueryDto
                {
                    Categoria = categoria.Descricao,
                    DespesaGrupoFatura1 = despesas
                        .Where(d =>
                            d.GrupoFaturaCode == grupoFaturaCode1 && d.CategoriaId == categoria.Id
                        )
                        .Sum(d => d.Total),
                    DespesaGrupoFatura2 = despesas
                        .Where(d =>
                            d.GrupoFaturaCode == grupoFaturaCode2 && d.CategoriaId == categoria.Id
                        )
                        .Sum(d => d.Total)
                })
                .ToList();

            return resultado;
        }
    }
}
