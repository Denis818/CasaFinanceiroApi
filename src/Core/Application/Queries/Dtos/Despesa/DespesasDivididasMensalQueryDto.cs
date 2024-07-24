using Domain.Dtos.QueryResults.Despesas;

namespace Application.Queries.Dtos
{
    public class DespesasDivididasMensalQueryDto
    {
        public RelatorioGastosDoGrupoResult RelatorioGastosDoGrupo { get; set; }
        public IEnumerable<DespesaPorMembroQueryDto> DespesasPorMembro { get; set; }
    }
}
