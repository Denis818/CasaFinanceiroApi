namespace Domain.Utilities
{
    public class PagedResult<T>
    {
        public int TotalItens { get; set; }
        public int PaginaAtual { get; set; }
        public List<T> Itens { get; set; } = [];
    }
}
