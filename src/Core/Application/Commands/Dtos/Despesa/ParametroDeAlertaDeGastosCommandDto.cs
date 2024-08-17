namespace Application.Commands.Dtos
{
    public class ParametroDeAlertaDeGastosCommandDto
    {
        public string TipoMetrica { get; set; }
        public decimal LimiteVermelho { get; set; }
        public decimal LimiteAmarelo { get; set; }
    }
}
