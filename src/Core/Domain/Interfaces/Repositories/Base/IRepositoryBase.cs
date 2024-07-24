using System.Linq.Expressions;

namespace Domain.Interfaces.Repositories.Base
{
    public interface IRepositoryBase<TEntity> where TEntity : class, new()
    {
        void Delete(TEntity entity);
        void DeleteRange(TEntity[] entityArray);
        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> expression = null);
        Task<TEntity> GetByIdAsync(int id);
        Task InsertAsync(TEntity entity);
        Task InsertRangeAsync(List<TEntity> entity);
        Task<bool> SaveChangesAsync();
        void Update(TEntity entity);
        Task<List<T>> ExecuteSqlRawAsync<T>(string sql, params object[] parameters) where T : class, new();
    }
}