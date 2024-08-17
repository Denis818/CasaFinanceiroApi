using Domain.Models.Membros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Mappings.Membros
{
    internal class MembroMap : IEntityTypeConfiguration<Membro>
    {
        public void Configure(EntityTypeBuilder<Membro> builder)
        {
            builder.ToTable("Membros");
            builder.Property(m => m.Id).IsRequired().HasColumnType("int").ValueGeneratedOnAdd();
            builder.Property(c => c.Code).IsRequired().HasColumnType("char(36)").IsRequired().ValueGeneratedOnAdd();

            builder.Property(d => d.DataInicio).HasColumnName("Data_Inicio").HasColumnType("datetime(6)").IsRequired();
            builder.Property(m => m.Nome).HasColumnType("varchar(30)").IsRequired();

            builder
                .Property(u => u.Telefone)
                .HasColumnName("Telefone")
                .HasColumnType("varchar(20)")
                .HasMaxLength(20)
                .IsRequired();

            builder.HasIndex(m => m.Nome).HasDatabaseName("IX_Membros_Nome");
            builder.HasIndex(m => m.DataInicio).HasDatabaseName("IX_Membros_Data_Inicio");
        }
    }
}
