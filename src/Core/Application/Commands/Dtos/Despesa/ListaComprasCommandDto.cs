using Application.Commands.Services.Base;

namespace Application.Commands.Dtos.Despesa
{
    public class ListaComprasCommandDto : CommandBaseDTO
    {
        public string Item { get; set; }
    }
}
