using Domain.Models.Base;
using Domain.Models.Despesas;
using System.Text.Json.Serialization;

namespace Domain.Models.Categorias
{
    public class Categoria : EntityBase
    {
        public string Descricao { get; set; }

        [JsonIgnore]
        public ICollection<Despesa> Despesas { get; set; } = [];
    }
}
