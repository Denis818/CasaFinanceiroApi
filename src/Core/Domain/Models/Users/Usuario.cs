namespace Domain.Models.Users
{
    public class Usuario
    {
        public Usuario()
        {
            Code = Guid.NewGuid();
        }

        public int Id { get; set; }
        public Guid Code { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public virtual ICollection<Permissao> Permissoes { get; set; } = [];
    }
}
