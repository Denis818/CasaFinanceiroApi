using Application.Commands.Services.Base;

namespace Application.Commands.Dtos
{
    public class ParametroDeAlertaDeGastosCommandDto : CommandBaseDTO
    {
        public string TipoMetrica { get; set; }
        public decimal LimiteVermelho { get; set; }
        public decimal LimiteAmarelo { get; set; }
    }
}
