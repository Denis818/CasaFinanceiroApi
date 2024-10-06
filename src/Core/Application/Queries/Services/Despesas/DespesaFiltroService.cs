using Application.Queries.Interfaces.Despesas;
using Domain.Enumeradores;
using Domain.Models.Despesas;

namespace Application.Queries.Services.Despesas
{
    public class DespesaFiltroService : IDespesaFiltroService
    {
        public IOrderedQueryable<Despesa> GetDespesasFiltradas(IQueryable<Despesa> query,
            string filter, EnumFiltroDespesa tipoFiltro)
        {
            filter = filter.ToLower();

            query = tipoFiltro switch
            {
                EnumFiltroDespesa.Item => query.Where(despesa => despesa.Item.ToLower().Contains(filter)),
                EnumFiltroDespesa.Fornecedor => query.Where(despesa => despesa.Fornecedor.ToLower().Contains(filter)),
                EnumFiltroDespesa.GrupoFatura => query.Where(despesa => despesa.GrupoFatura.Nome.ToLower().Contains(filter)),
                EnumFiltroDespesa.Categoria => query.Where(despesa => despesa.Categoria.Descricao.ToLower().Contains(filter)),

                EnumFiltroDespesa.Preco or
                EnumFiltroDespesa.PrecoMenorOuIgual or
                EnumFiltroDespesa.PrecoMaiorOuIgual => FilterByPrice(query, filter, tipoFiltro),

                _ => query
            };

            return query.OrderByDescending(d => d.DataCompra);
        }

        private IQueryable<Despesa> FilterByPrice(IQueryable<Despesa> query,
            string filter, EnumFiltroDespesa tipoFiltro)
        {
            if (double.TryParse(filter.Replace(",", "."), out double precoValue))
            {
                query = tipoFiltro switch
                {
                    EnumFiltroDespesa.Preco => query.Where(despesa => despesa.Preco == precoValue),
                    EnumFiltroDespesa.PrecoMenorOuIgual => query.Where(despesa => despesa.Preco <= precoValue),
                    EnumFiltroDespesa.PrecoMaiorOuIgual => query.Where(despesa => despesa.Preco >= precoValue),
                    _ => query
                };
            }

            return query;
        }
    }
}
