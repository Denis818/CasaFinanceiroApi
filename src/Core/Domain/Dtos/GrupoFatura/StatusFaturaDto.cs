using Domain.Dtos.Base;

namespace Domain.Dtos
{
    public class StatusFaturaDto : BaseDto
    {
        public string FaturaNome { get; set; }
        public string Estado { get; set; }
    }
}
