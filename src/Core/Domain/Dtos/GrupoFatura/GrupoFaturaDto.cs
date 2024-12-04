using Domain.Converters.DatesTimes;
using Domain.Dtos.Base;
using System.Text.Json.Serialization;

namespace Domain.Dtos
{
    public class GrupoFaturaDto : BaseDto
    {
        public string Nome { get; set; }
        public string Ano { get; set; }
        public int QuantidadeDespesas { get; set; }
        public double TotalDespesas { get; set; }
        public double? Desconto { get; set; }
        public IList<StatusFaturaDto> StatusFaturas { get; set; } = [];

        [JsonConverter(typeof(LongDateFormatConverter))]
        public DateTime DataCriacao { get; set; }
    }
}
