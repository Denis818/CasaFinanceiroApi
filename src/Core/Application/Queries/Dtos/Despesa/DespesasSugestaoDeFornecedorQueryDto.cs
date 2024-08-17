using Domain.Dtos;
using Domain.Utilities;

namespace Application.Queries.Dtos
{
    public class DespesasSugestaoDeFornecedorQueryDto
    {
        public string Sugestao { get; set; }
        public PagedResult<DespesaQueryDto> ListaItens { get; set; }
    }
}
