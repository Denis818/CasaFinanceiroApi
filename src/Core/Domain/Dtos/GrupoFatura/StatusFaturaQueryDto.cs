using Domain.Dtos.Base;

namespace Domain.Dtos
{
    public class StatusFaturaQueryDto : QueryBaseDTO
    {
        public string FaturaNome { get; set; }
        public string Estado { get; set; }
    }
}
