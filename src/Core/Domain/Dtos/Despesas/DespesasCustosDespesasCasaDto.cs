namespace Domain.Dtos.Despesas
{
    public class DespesasCustosDespesasCasaDto
    {
        public List<MembroDto> TodosMembros { get; set; }
        public double ValorTotalAlmoco { get; set; }
        public double TotalDespesaGeraisForaAlmoco { get; set; }
        public int MembrosForaJhonCount { get; set; }
    }
}
