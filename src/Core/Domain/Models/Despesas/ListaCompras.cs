using Domain.Models.Base;

namespace Domain.Models.Despesas
{
    public class ListaCompras : EntityBase
    {
        public int Id { get; set; }
        public string Item { get; set; }
    }
}
