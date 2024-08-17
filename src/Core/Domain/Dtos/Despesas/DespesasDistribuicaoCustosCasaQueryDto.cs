namespace Domain.Dtos.Despesas
{
    public class DespesasDistribuicaoCustosCasaQueryDto
    {
        public List<MembroQueryDto> Membros { get; set; }
        public double TotalDespesaGeraisForaAlmoco { get; set; }
        public double TotalAlmocoDividioComJhon { get; set; }
        public double TotalAlmocoParteDoJhon { get; set; }
        public double TotalDespesasGeraisMaisAlmocoDividido { get; set; }
        public double TotalSomenteAlmoco { get; set; }
        public double DespesaGeraisMaisAlmoco { get; set; }
        public double DespesaGeraisMaisAlmocoDividioPorMembro { get; set; }
    }
}
