using Application.Configurations.MappingsApp;
using Application.Queries.Dtos;
using Application.Queries.Interfaces.Despesa;
using Application.Queries.Services.Base;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Services
{
    public class ProdutoListaComprasQueryService(IServiceProvider service)
        : BaseQueryService<ProdutoListaCompras, ProdutoListaComprasQueryDto, IProdutoListaComprasRepository>(service), IProdutoListaComprasQueryService
    {
        protected override ProdutoListaComprasQueryDto MapToDTO(ProdutoListaCompras entity) => entity.MapToDTO();

        public async Task<IEnumerable<ProdutoListaComprasQueryDto>> GetAllAsync() =>
            await _repository
                .Get()
                .OrderBy(c => c.Item)
                .AsNoTracking()
                .Select(m => m.MapToDTO())
                .ToListAsync();
    }
}
