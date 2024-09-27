using Application.Queries.Interfaces.Despesas;
using Domain.Enumeradores;
using Domain.Models.Despesas;

namespace Application.Queries.Services.Despesas
{
    public class DespesaFiltroService : IDespesaFiltroService
    {
        public IOrderedQueryable<Despesa> GetDespesasFiltradas(
            IQueryable<Despesa> query,
            string filter,
            EnumFiltroDespesa tipoFiltro
        )
        {
            switch (tipoFiltro)
            {
                case EnumFiltroDespesa.Item:
                query = query.Where(despesa =>
                    despesa.Item.ToLower().Contains(filter.ToLower())
                );
                break;

                case EnumFiltroDespesa.Categoria:
                query = query.Where(despesa =>
                    despesa.Categoria.Descricao.ToLower().Contains(filter.ToLower())
                );
                break;

                case EnumFiltroDespesa.Fornecedor:
                query = query.Where(despesa =>
                    despesa.Fornecedor.ToLower().Contains(filter.ToLower())
                );
                break;

                case EnumFiltroDespesa.GrupoFatura:
                query = query.Where(despesa =>
                    despesa.GrupoFatura.Nome.ToLower().Contains(filter.ToLower())
                );
                break;

                case EnumFiltroDespesa.Preco:
                if (double.TryParse(filter, out double precoValue))
                {
                    string filterPreco = filter.Replace(",", ".");
                    query = query.Where(despesa => despesa.Preco == precoValue);
                }
                break;

                case EnumFiltroDespesa.PrecoMenorOuIgual:
                if (double.TryParse(filter, out double precoMenorIgualValue))
                {
                    string filterPreco = filter.Replace(",", ".");
                    query = query.Where(despesa => despesa.Preco <= precoMenorIgualValue);
                }
                break;

                case EnumFiltroDespesa.PrecoMaiorOuIgual:
                if (double.TryParse(filter, out double precoMaiorOuIgualValue))
                {
                    string filterPreco = filter.Replace(",", ".");
                    query = query.Where(despesa => despesa.Preco >= precoMaiorOuIgualValue);
                }
                break;
            }

            return query.OrderByDescending(d => d.DataCompra);
        }
    }
}
