using Domain.Models.Categorias;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Data.Mappings.Categorias
{
    internal class CategoriaMap : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.ToTable("Categorias");

            builder.HasKey(e => new { e.Code });

            builder.Property(c => c.Code).IsRequired().HasColumnType("char(36)").IsRequired().ValueGeneratedOnAdd();

            builder.Property(c => c.Descricao).HasColumnType("varchar(50)").IsRequired();

            builder.HasIndex(c => c.Descricao).HasDatabaseName("IX_Categorias_Descricao");
            builder.HasIndex(c => c.Code).HasDatabaseName("IX_Categorias_Code");
        }
    }
}
