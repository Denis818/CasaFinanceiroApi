using Domain.Enumeradores;

namespace Application.Queries.Dtos
{
    public class DespesaFiltroDto
    {
        public string Filter { get; set; }
        public int PaginaAtual { get; set; } = 1;
        public int ItensPorPagina { get; set; } = 10;
        public EnumFiltroDespesa TipoFiltro { get; set; } = EnumFiltroDespesa.Item;
    }
}
