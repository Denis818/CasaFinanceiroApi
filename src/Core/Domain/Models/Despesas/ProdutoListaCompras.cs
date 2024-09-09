using Domain.Models.Base;

namespace Domain.Models.Despesas
{
    public class ProdutoListaCompras : EntityBase
    {
        public int Id { get; set; }
        public string Item { get; set; }
    }
}
