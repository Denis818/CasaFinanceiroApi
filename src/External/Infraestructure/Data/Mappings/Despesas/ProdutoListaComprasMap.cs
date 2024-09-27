using Domain.Models.ListaCompras;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Data.Mappings.Despesas
{
    internal class ProdutoListaComprasMap : IEntityTypeConfiguration<ProdutoListaCompras>
    {
        public void Configure(EntityTypeBuilder<ProdutoListaCompras> builder)
        {
            builder.ToTable("Produto_Lista_Compras");

            builder.HasKey(e => new { e.Id });

            builder.Property(d => d.Id).HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            builder.Property(c => c.Code).IsRequired().HasColumnType("char(36)").IsRequired().ValueGeneratedOnAdd();

            builder.Property(d => d.Item).HasColumnType("varchar(50)").IsRequired();

            builder.HasIndex(c => c.Code).HasDatabaseName("IX_Produto_Lista_Compras_Code");
            builder.HasIndex(c => c.Id).HasDatabaseName("IX_Produto_Lista_Compras_Id");
            builder.HasIndex(c => c.Item).HasDatabaseName("IX_Produto_Lista_Compras_Item");
        }
    }
}
