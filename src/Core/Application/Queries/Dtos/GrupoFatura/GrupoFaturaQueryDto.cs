using Domain.Models.Despesas;
using System.Text.Json.Serialization;

namespace Application.Queries.Dtos
{
    public class GrupoFaturaQueryDto
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
