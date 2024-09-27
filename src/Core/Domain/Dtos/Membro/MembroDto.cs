using Domain.Converters.DatesTimes;
using Domain.Dtos.Base;
using System.Text.Json.Serialization;

namespace Domain.Dtos
{
    public class MembroDto : BaseDto
    {
        public string Nome { get; set; }
        public string Telefone { get; set; }

        [JsonConverter(typeof(LongDateFormatConverter))]
        public DateTime DataInicio { get; set; }
    }
}
