using Domain.Dtos.Despesas;

namespace Application.Queries.Interfaces.Despesa
{
    public interface IListaComprasQueryService
    {
        Task<IEnumerable<ListaComprasQueryDto>> GetAllAsync();
    }
}