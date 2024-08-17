using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Mappings.User
{
    public class PermissaoMap : IEntityTypeConfiguration<Permissao>
    {
        public void Configure(EntityTypeBuilder<Permissao> builder)
        {
            builder.ToTable("Permissoes");
            builder.Property(p => p.Id).IsRequired().HasColumnType("int").ValueGeneratedOnAdd();
            builder.Property(c => c.Code).IsRequired().HasColumnType("char(36)").IsRequired().ValueGeneratedOnAdd();

            builder.Property(p => p.Descricao).HasColumnType("varchar(30)").IsRequired();

            builder.HasIndex(p => p.Descricao).HasDatabaseName("IX_Permissoes_Descricao");
        }
    }
}
