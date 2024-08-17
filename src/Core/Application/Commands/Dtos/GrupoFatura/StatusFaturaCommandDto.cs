using Application.Commands.Services.Base;

namespace Application.Commands.Dtos
{
    public class StatusFaturaCommandDto : CommandBaseDTO
    {

        public string FaturaNome { get; set; }
        public string Estado { get; set; }
    }
}
