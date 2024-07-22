namespace Application.Queries.Dtos
{
    public class DespesasDivididasMensalQueryDto
    {
        public DespesasRelatorioGastosDoGrupoQueryDto RelatorioGastosDoGrupo { get; set; }
        public IEnumerable<DespesaPorMembroQueryDto> DespesasPorMembro { get; set; }
    }
}
