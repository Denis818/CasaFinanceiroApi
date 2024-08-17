using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Data.Mappings.Despesas
{
    internal class ParametroDeAlertaDeGastosMap : IEntityTypeConfiguration<ParametroDeAlertaDeGastos>
    {
        public void Configure(EntityTypeBuilder<ParametroDeAlertaDeGastos> builder)
        {
            builder.ToTable("Parametro_Alerta_Gastos");

            builder.HasKey(mp => mp.Id);
            builder.Property(c => c.Code).IsRequired().HasColumnType("char(36)").ValueGeneratedOnAdd();

            builder.Property(mp => mp.TipoMetrica)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(mp => mp.LimiteVermelho)
                .IsRequired()
                .HasColumnType("decimal(8,2)");

            builder.Property(mp => mp.LimiteAmarelo)
                .IsRequired()
                .HasColumnType("decimal(8,2)");


            builder.HasIndex(c => c.Id).HasDatabaseName("IX_Parametro_Alerta_Gastos_Id");
            builder.HasIndex(c => c.TipoMetrica).HasDatabaseName("IX_Parametro_Alerta_Gastos_TipoMetrica");
        }
    }
}