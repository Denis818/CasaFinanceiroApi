using Domain.Converters.DatesTimes;
using System.Text.Json.Serialization;

namespace Domain.Models.Despesas
{
    public class GrupoFatura
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public string Ano { get; set; }

        [JsonConverter(typeof(LongDateFormatConverter))]
        public DateTime DataCriacao { get; set; }

        [JsonIgnore]
        public ICollection<Despesa> Despesas { get; set; } = [];
        public ICollection<StatusFatura> StatusFaturas { get; set; } = [];
    }

}
