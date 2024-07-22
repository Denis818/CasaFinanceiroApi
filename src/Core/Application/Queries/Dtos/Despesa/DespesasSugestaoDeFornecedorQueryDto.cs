using Domain.Models.Despesas;
using Domain.Utilities;

namespace Application.Queries.Dtos
{
    public class DespesasSugestaoDeFornecedorQueryDto
    {
        public string Sugestao { get; set; }
        public PagedResult<Despesa> ListaItens { get; set; }
    }
}
