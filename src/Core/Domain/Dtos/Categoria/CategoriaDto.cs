using Domain.Dtos.Base;

namespace Domain.Dtos.Categoria
{
    public class CategoriaDto : BaseDto
    {
        public string Descricao { get; set; }
        public double Total { get; set; }
        public int QuantidadeDeItens { get; set; }
    }
}
