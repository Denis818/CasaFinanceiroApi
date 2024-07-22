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
            builder.Property(c => c.Id).IsRequired().HasColumnType("int").ValueGeneratedOnAdd();

            builder.Property(d => d.DataCriacao).HasColumnName("Data_Criacao").HasColumnType("datetime(6)").IsRequired();
            builder.Property(c => c.Nome).HasColumnType("varchar(30)").IsRequired();
            builder.Property(c => c.Ano).HasColumnType("varchar(15)").IsRequired();

            builder.HasIndex(c => c.Id).HasDatabaseName("IX_Grupo_Fatura_Id");
            builder.HasIndex(c => c.Nome).HasDatabaseName("IX_Grupo_Fatura_Nome");
            builder.HasIndex(c => c.Ano).HasDatabaseName("IX_Grupo_Fatura_Ano");
        }
    }
}
