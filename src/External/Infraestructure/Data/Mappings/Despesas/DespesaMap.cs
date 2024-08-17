using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Data.Mappings.Despesas
{
    internal class DespesaMap : IEntityTypeConfiguration<Despesa>
    {
        public void Configure(EntityTypeBuilder<Despesa> builder)
        {
            builder.ToTable("Despesas");
            builder.Property(d => d.Id).HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            builder.Property(c => c.Code).IsRequired().HasColumnType("char(36)").IsRequired().ValueGeneratedOnAdd();

            builder.Property(d => d.DataCompra).HasColumnType("datetime(6)").IsRequired();
            builder.Property(d => d.Item).HasColumnType("varchar(50)").IsRequired();
            builder.Property(d => d.Preco).HasColumnType("double(7, 2)").IsRequired();
            builder.Property(d => d.Quantidade).HasColumnType("int").IsRequired();
            builder.Property(d => d.Fornecedor).HasColumnType("varchar(50)").IsRequired();
            builder.Property(d => d.Total).HasColumnType("double(7, 2)").IsRequired();
            builder.Property(d => d.CategoriaId).HasColumnType("int").IsRequired();
            builder.Property(d => d.GrupoFaturaId).HasColumnType("int").IsRequired();

            builder.Property(d => d.CategoriaCode).HasColumnType("char(36)").IsRequired();
            builder.Property(d => d.GrupoFaturaCode).HasColumnType("char(36)").IsRequired();

            builder
                .HasOne(d => d.Categoria)
                .WithMany(c => c.Despesas)
                .HasForeignKey(d => d.CategoriaId);

            builder
                .HasOne(d => d.GrupoFatura)
                .WithMany(c => c.Despesas)
                .HasForeignKey(d => d.GrupoFaturaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(d => d.Id).HasDatabaseName("IX_Despesas_Id");
            builder.HasIndex(d => d.Item).HasDatabaseName("IX_Despesas_Item");
            builder.HasIndex(d => d.DataCompra).HasDatabaseName("IX_Despesas_DataCompra");
            builder.HasIndex(d => d.Fornecedor).HasDatabaseName("IX_Despesas_Fornecedor");
            builder.HasIndex(d => d.GrupoFaturaId).HasDatabaseName("IX_Despesas_GrupoFaturaId");

        }
    }
}
