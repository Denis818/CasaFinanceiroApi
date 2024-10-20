using Domain.Models.Base;

namespace Domain.Models.Compras
{
    public class Compra : EntityBase
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Parcelas { get; set; }
        public double ValorTotal { get; set; }
        public double ValorPorParcela { get; set; }
        public bool DividioPorDois { get; set; }
    }
}
