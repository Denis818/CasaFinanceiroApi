using Domain.Converters.DatesTimes;
using Domain.Dtos.Base;
using System.Text.Json.Serialization;

namespace Domain.Dtos
{
    public class GrupoFaturaQueryDto : QueryBaseDTO
    {
        public string Nome { get; set; }
        public string Ano { get; set; }
        public int QuantidadeDespesas { get; set; }
        public double TotalDespesas { get; set; }
        public IList<StatusFaturaQueryDto> StatusFaturas { get; set; } = [];

        [JsonConverter(typeof(LongDateFormatConverter))]
        public DateTime DataCriacao { get; set; }
    }
}
