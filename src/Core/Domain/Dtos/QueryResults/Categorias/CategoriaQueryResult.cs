using Domain.Dtos.Base;

namespace Domain.Dtos.QueryResults
{
    public class CategoriaQueryResult : BaseDto
    {
        public string Descricao { get; set; }
        public double Total { get; set; }
        public int QuantidadeDeItens { get; set; }
    }
}
