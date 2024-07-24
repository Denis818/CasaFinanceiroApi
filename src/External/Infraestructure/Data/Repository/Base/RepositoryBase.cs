using Domain.Interfaces.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace Data.Repository.Base
{
    public abstract class RepositoryBase<TEntity, TContext> : IRepositoryBase<TEntity> where TEntity : class, new()
        where TContext : DbContext
    {
        private readonly TContext _context;
        private DbSet<TEntity> DbSet { get; }

        protected RepositoryBase(IServiceProvider service)
        {
            _context = service.GetRequiredService<TContext>();
            DbSet = _context.Set<TEntity>();
        }

        public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> expression = null)
        {
            if (expression != null)
                return DbSet.Where(expression);

            return DbSet.AsNoTracking();
        }

        public async Task<TEntity> GetByIdAsync(int id) => await DbSet.FindAsync(id);

        public virtual async Task InsertAsync(TEntity entity) => await DbSet.AddAsync(entity);

        public async Task InsertRangeAsync(List<TEntity> entity) =>
            await DbSet.AddRangeAsync(entity);

        public void Update(TEntity entity) => DbSet.Update(entity);

        public void Delete(TEntity entity) => DbSet.Remove(entity);

        public void DeleteRange(TEntity[] entityArray) => DbSet.RemoveRange(entityArray);

        public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;

        public async Task<List<T>> ExecuteSqlRawAsync<T>(string sql, params object[] parameters) where T : class, new()
        {
            return await _context.Set<T>().FromSqlRaw(sql, parameters).ToListAsync();
        }

    }
}
