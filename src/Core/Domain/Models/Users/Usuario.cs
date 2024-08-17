using Domain.Models.Base;

namespace Domain.Models.Users
{
    public class Usuario : EntityBase
    {
        public int Id { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public virtual ICollection<Permissao> Permissoes { get; set; } = [];
    }
}
