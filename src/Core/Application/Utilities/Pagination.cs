using Domain.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Application.Utilities
{
    public class Pagination
    {
        public static async Task<PagedResult<T>> PaginateResultAsync<T>(
            IQueryable<T> consulta,
            int numeroPagina = 1,
            int tamanhoPagina = 10
        )
            where T : class
        {
            int totalItens = await consulta.CountAsync();
            int quantidadePular = 0;

            if (totalItens > tamanhoPagina)
            {
                quantidadePular = (numeroPagina - 1) * tamanhoPagina;
            }

            var itens = await consulta.Skip(quantidadePular).Take(tamanhoPagina).ToListAsync();

            return new PagedResult<T>
            {
                TotalItens = totalItens,
                PaginaAtual = numeroPagina,
                Itens = itens,
            };
        }

        public static PagedResult<T> PaginateResult<T>(
            IList<T> list,
            int numeroPagina = 1,
            int tamanhoPagina = 10
        )
            where T : class
        {
            int totalItens = list.Count;
            int quantidadePular = 0;

            if (totalItens > tamanhoPagina)
            {
                quantidadePular = (numeroPagina - 1) * tamanhoPagina;
            }

            var itens = list.Skip(quantidadePular).Take(tamanhoPagina).ToList();

            return new PagedResult<T>
            {
                TotalItens = totalItens,
                PaginaAtual = numeroPagina,
                Itens = itens,
            };
        }
    }
}
