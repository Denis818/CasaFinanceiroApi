using Domain.Models.Despesas;
using System.Text.Json.Serialization;

namespace Domain.Dtos.QueryResults
{
    public class GrupoFaturaQueryResult
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Ano { get; set; }
        public int QuantidadeDespesas { get; set; }
        public double TotalDespesas { get; set; }
        public ICollection<StatusFatura> StatusFaturas { get; set; } = [];

        [JsonIgnore]
        public ICollection<Despesa> Despesas { get; set; } = [];
    }
}
