namespace Domain.Models.Users
{
    public class Permissao
    {
        public Permissao()
        {
            Code = Guid.NewGuid();
        }

        public int Id { get; set; }
        public Guid Code { get; set; }

        public string Descricao { get; set; }
        public virtual ICollection<Usuario> Usuarios { get; set; } = [];
    }
}
