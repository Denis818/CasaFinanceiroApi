using Domain.Dtos;
using Domain.Dtos.QueryResults.Despesas;
using Domain.Models.Categorias;
using Domain.Models.Despesas;
using Domain.Models.Membros;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace Data.DataContext
{
    public partial class FinanceDbContext(DbContextOptions<FinanceDbContext> options)
        : DbContext(options)
    {
        #region Models
        public DbSet<Despesa> Despesas { get; set; }
        public DbSet<GrupoFatura> GrupoFaturas { get; set; }
        public DbSet<StatusFatura> StatusFaturas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Membro> Membros { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Permissao> Permissoes { get; set; }
        #endregion

        #region QueryResults
        public DbSet<RelatorioGastosDoGrupoResult> DespesasRelatorioGastosDoGrupo { get; set; }
        public DbSet<DespesasPorGrupoResult> DespesasPorGrupoQueryDto { get; set; }
        public DbSet<TotalPorCategoriaQueryResut> DespesasTotalPorCategoria { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureQueryResults(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceDbContext).Assembly);
        }

        public void SetConnectionString(string newStringConnection)
        {
            Database.CurrentTransaction?.Commit();

            Database.SetConnectionString(newStringConnection);
        }

        private void ConfigureQueryResults(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RelatorioGastosDoGrupoResult>().HasNoKey();
            modelBuilder.Entity<DespesasPorGrupoResult>().HasNoKey();
            modelBuilder.Entity<TotalPorCategoriaQueryResut>().HasNoKey();
        }
    }
}