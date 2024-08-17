using Application.Commands.Services.Base;
using Domain.Converters.DatesTimes;
using System.Text.Json.Serialization;

namespace Application.Commands.Dtos
{
    public class MembroCommandDto : CommandBaseDTO
    {
        public string Nome { get; set; }
        public string Telefone { get; set; }

        [JsonConverter(typeof(LongDateFormatConverter))]
        public DateTime DataInicio { get; set; }
    }
}
