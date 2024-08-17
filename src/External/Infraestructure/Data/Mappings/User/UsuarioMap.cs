using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Mappings.User
{
    public class UsuarioMap : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");
            builder.Property(u => u.Id).IsRequired().HasColumnType("int").ValueGeneratedOnAdd();
            builder.Property(c => c.Code).IsRequired().HasColumnType("char(36)").ValueGeneratedOnAdd();

            builder.Property(u => u.Email).HasColumnType("varchar(30)").IsRequired();
            builder.Property(u => u.Password).HasColumnType("varchar(100)").IsRequired();
            builder.Property(u => u.Salt).HasColumnType("varchar(100)").IsRequired();

            builder
                .HasMany(u => u.Permissoes)
                .WithMany(p => p.Usuarios)
                .UsingEntity<Dictionary<string, object>>(
                    "Usuario_Permissao",
                    column => column.HasOne<Permissao>().WithMany().HasForeignKey("Permissao_Id"),
                    column =>
                        column.HasOne<Usuario>().WithMany().HasForeignKey("Usuario_Id"),
                    table =>
                    {
                        table.ToTable("Usuario_Permissao");
                        table.HasKey("Usuario_Id", "Permissao_Id");
                    }
                );

            builder.HasIndex(u => u.Id).HasDatabaseName("IX_Usuarios_Id");
            builder.HasIndex(u => u.Email).HasDatabaseName("IX_Email");
        }
    }
}
