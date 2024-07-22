using Domain.Models.Membros;

namespace Domain.Dtos.Despesas
{
    public class DespesasCustosDespesasCasaQueryDto
    {
        public List<Membro> TodosMembros { get; set; }
        public double ValorTotalAlmoco { get; set; }
        public double TotalDespesaGeraisForaAlmoco { get; set; }
        public int MembrosForaJhonCount { get; set; }
    }
}
