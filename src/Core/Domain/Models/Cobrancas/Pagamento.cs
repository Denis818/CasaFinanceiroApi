using Domain.Converters.DatesTimes;
using Domain.Models.Base;
using System.Text.Json.Serialization;

namespace Domain.Models.Cobrancas
{
    public class Pagamento : EntityBase
    {
        public int Id { get; set; }

        [JsonConverter(typeof(ShortDateFormatConverter))]
        public DateTime Data { get; set; }
        public double Valor { get; set; }
    }
}
