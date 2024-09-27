namespace Application.Queries.Dtos
{
    public class DespesasSugestaoEconomiaQueryDto
    {
        public string Item { get; set; }
        public string FornecedorMaisBarato { get; set; }
        public double PrecoMaisBarato { get; set; }
        public double PotencialEconomia { get; set; }
    }
}
