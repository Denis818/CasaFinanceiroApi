using Domain.Models.Base;

namespace Domain.Models.Despesas
{
    public class GrupoFatura : EntityBase
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Ano { get; set; }
        public ICollection<StatusFatura> StatusFaturas { get; set; } = [];
        public ICollection<Despesa> Despesas { get; set; } = [];
        public DateTime DataCriacao { get; set; }
    }
}
