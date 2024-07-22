namespace Application.Commands.Dtos
{
    public class DespesaCommandDto
    {
        public int GrupoFaturaId { get; set; }
        public int CategoriaId { get; set; }
        public string Item { get; set; }
        public double Preco { get; set; }
        public int Quantidade { get; set; }
        public string Fornecedor { get; set; }
    }
}
