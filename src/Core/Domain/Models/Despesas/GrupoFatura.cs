using Domain.Converters.DatesTimes;
using System.Text.Json.Serialization;

namespace Domain.Models.Despesas
{
    public class GrupoFatura
    {
        public GrupoFatura()
        {
            Code = Guid.NewGuid();
        }

        public int Id { get; set; }
        public Guid Code { get; set; }

        public string Nome { get; set; }
        public string Ano { get; set; }
        public ICollection<StatusFatura> StatusFaturas { get; set; } = [];

        [JsonIgnore]
        public ICollection<Despesa> Despesas { get; set; } = [];

        [JsonConverter(typeof(LongDateFormatConverter))]
        public DateTime DataCriacao { get; set; }
    }
}
