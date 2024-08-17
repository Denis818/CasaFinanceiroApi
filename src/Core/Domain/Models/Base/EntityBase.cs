namespace Domain.Models.Base
{
    public abstract class EntityBase
    {
        public Guid Code { get; set; }

        public EntityBase()
        {
            Code = Guid.NewGuid();
        }
    }
}
