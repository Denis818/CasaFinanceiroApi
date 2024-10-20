using Domain.Models.Compras;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Data.Mappings.Compras
{
    internal class RecebimentoMapping : IEntityTypeConfiguration<Recebimento>
    {
        public void Configure(EntityTypeBuilder<Recebimento> builder)
        {
            builder.ToTable("Recebimentos");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id).HasColumnType("int").IsRequired();

            builder
                .Property(c => c.Code)
                .IsRequired()
                .HasColumnType("char(36)")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(r => r.Data).HasColumnType("datetime(6)").IsRequired();

            builder.Property(r => r.Valor).HasColumnType("double(7,2)").IsRequired();
        }
    }
}
