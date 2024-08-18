using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Data.Mappings.Despesas
{
    internal class GrupoFaturaMap : IEntityTypeConfiguration<GrupoFatura>
    {
        public void Configure(EntityTypeBuilder<GrupoFatura> builder)
        {
            builder.ToTable("Grupo_Fatura");

            builder.HasKey(e => new { e.Code });

            builder.Property(c => c.Code).IsRequired().HasColumnType("char(36)").IsRequired().ValueGeneratedOnAdd();

            builder.Property(d => d.DataCriacao).HasColumnName("Data_Criacao").HasColumnType("datetime(6)").IsRequired();
            builder.Property(c => c.Nome).HasColumnType("varchar(30)").IsRequired();
            builder.Property(c => c.Ano).HasColumnType("varchar(15)").IsRequired();

            builder.HasIndex(c => c.Code).HasDatabaseName("IX_Grupo_Fatura_Code");
            builder.HasIndex(c => c.Nome).HasDatabaseName("IX_Grupo_Fatura_Nome");
            builder.HasIndex(c => c.Ano).HasDatabaseName("IX_Grupo_Fatura_Ano");
        }
    }
}
