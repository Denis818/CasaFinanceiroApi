using Domain.Models.Cobrancas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Data.Mappings.Compras
{
    internal class CompraMapping : IEntityTypeConfiguration<Cobranca>
    {
        public void Configure(EntityTypeBuilder<Cobranca> builder)
        {
            builder.ToTable("Cobrancas");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id).HasColumnType("int").IsRequired();

            builder
                .Property(c => c.Code)
                .IsRequired()
                .HasColumnType("char(36)")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(c => c.Nome).HasColumnType("varchar(70)").IsRequired();

            builder.Property(c => c.Parcelas).HasColumnType("int").IsRequired();

            builder.Property(c => c.ValorTotal).HasColumnType("double(7,2)").IsRequired();
            builder.Property(c => c.ValorPorParcela).HasColumnType("double(7,2)").IsRequired();
            builder.Property(c => c.DividioPorDois).HasColumnType("TINYINT(1)").IsRequired();
        }
    }
}
