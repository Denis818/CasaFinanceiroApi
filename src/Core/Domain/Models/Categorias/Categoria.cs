using Domain.Models.Despesas;
using System.Text.Json.Serialization;

namespace Domain.Models.Categorias
{
    public class Categoria
    {
        public Categoria()
        {
            Code = Guid.NewGuid();
        }

        public int Id { get; set; }
        public Guid Code { get; set; }
        public string Descricao { get; set; }

        [JsonIgnore]
        public ICollection<Despesa> Despesas { get; set; } = [];
    }
}
