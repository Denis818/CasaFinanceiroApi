namespace Domain.Dtos.Despesas
{
    public class DespesasCustosDespesasCasaQueryDto
    {
        public List<MembroQueryDto> TodosMembros { get; set; }
        public double ValorTotalAlmoco { get; set; }
        public double TotalDespesaGeraisForaAlmoco { get; set; }
        public int MembrosForaJhonCount { get; set; }
    }
}
