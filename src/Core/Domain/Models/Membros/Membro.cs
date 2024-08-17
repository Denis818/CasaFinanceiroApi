using Domain.Models.Base;

namespace Domain.Models.Membros
{
    public class Membro : EntityBase
    {
        public int Id { get; set; }

        public string Nome { get; set; }
        public string Telefone { get; set; }

        public DateTime DataInicio { get; set; }
    }
}
