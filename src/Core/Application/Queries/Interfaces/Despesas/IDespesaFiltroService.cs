using Domain.Enumeradores;
using Domain.Models.Despesas;

namespace Application.Queries.Interfaces.Despesas
{
    public interface IDespesaFiltroService
    {
        IOrderedQueryable<Despesa> GetDespesasFiltradas(IQueryable<Despesa> query, string filter, EnumFiltroDespesa tipoFiltro);
    }
}