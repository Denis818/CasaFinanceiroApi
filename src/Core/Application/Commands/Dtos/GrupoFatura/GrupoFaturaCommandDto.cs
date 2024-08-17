using Application.Commands.Services.Base;

namespace Application.Commands.Dtos
{
    public class GrupoFaturaCommandDto : CommandBaseDTO
    {
        public string Nome { get; set; }
        public string Ano { get; set; }
    }
}
