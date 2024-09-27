using Domain.Dtos.Base;

namespace Application.Queries.Dtos
{
    public class ParametroDeAlertaDeGastosQueryDto : BaseDto
    {
        public string TipoMetrica { get; set; }
        public decimal LimiteVermelho { get; set; }
        public decimal LimiteAmarelo { get; set; }
    }
}
