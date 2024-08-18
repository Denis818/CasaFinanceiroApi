namespace Application.Commands.Dtos
{
    public class DespesaCommandDto
    {
        public string Item { get; set; }
        public double Preco { get; set; }
        public int Quantidade { get; set; }
        public string Fornecedor { get; set; }
        //      public GrupoFaturaQueryDto GrupoFatura { get; set; }
        public Guid GrupoFaturaCode { get; set; }
        public Guid CategoriaCode { get; set; }
        public int GrupoFaturaId { get; set; }
        public int CategoriaId { get; set; }
    }
}
