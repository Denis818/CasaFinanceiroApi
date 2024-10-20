using Domain.Models.Categorias;
using Domain.Models.Compras;
using Domain.Models.Despesas;
using Domain.Models.GrupoFaturas;
using Domain.Models.ListaCompras;
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
        public DbSet<ProdutoListaCompras> ProdutoListaCompras { get; set; }
        public DbSet<ParametroDeAlertaDeGastos> ParametroDeAlertaDeGastoss { get; set; }
        public DbSet<GrupoFatura> GrupoFaturas { get; set; }
        public DbSet<StatusFatura> StatusFaturas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Membro> Membros { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Permissao> Permissoes { get; set; }
        public DbSet<Recebimento> Recebimentos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceDbContext).Assembly);
        }

        public void SetConnectionString(string newStringConnection)
        {
            Database.CurrentTransaction?.Commit();

            Database.SetConnectionString(newStringConnection);
        }
    }
}