using Domain.Models.Base;

namespace Domain.Models.ListaCompras
{
    public class ProdutoListaCompras : EntityBase
    {
        public int Id { get; set; }
        public string Item { get; set; }
    }
}
