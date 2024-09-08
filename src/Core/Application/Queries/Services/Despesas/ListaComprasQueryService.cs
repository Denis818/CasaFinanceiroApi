using Application.Configurations.MappingsApp;
using Application.Queries.Interfaces.Despesa;
using Application.Queries.Services.Base;
using Domain.Dtos.Despesas;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Services
{
    public class ListaComprasQueryService(IServiceProvider service)
        : BaseQueryService<ListaCompras, ListaComprasQueryDto, IListaComprasRepository>(service), IListaComprasQueryService
    {
        protected override ListaComprasQueryDto MapToDTO(ListaCompras entity) => entity.MapToDTO();

        public async Task<IEnumerable<ListaComprasQueryDto>> GetAllAsync() =>
            await _repository
                .Get()
                .OrderBy(c => c.Item)
                .AsNoTracking()
                .Select(m => m.MapToDTO())
                .ToListAsync();
    }
}
