using Application.Commands.Services.Base;

namespace Application.Commands.Dtos.Despesa
{
    public class ProdutoListaComprasCommandDto : CommandBaseDTO
    {
        public string Item { get; set; }
    }
}
