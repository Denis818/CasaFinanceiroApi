namespace Application.Queries.Dtos
{
    public class DespesasDivididasMensalQueryDto
    {
        public IEnumerable<DespesaPorMembroQueryDto> DespesasPorMembro { get; set; }
    }
}