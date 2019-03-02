using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFinanceServer.Core
{
    public interface IRepository
    {
        Task<TEntity> GetById<T, TEntity>(T id) where TEntity : class, IUniqueObject<T>;
        Task<List<TEntity>> GetList<TEntity>() where TEntity : class;
        Task<TEntity> Add<TEntity>(TEntity entity) where TEntity : class;
        Task<TEntity> Update<TEntity>(TEntity entity) where TEntity : class;
        Task Delete<TEntity>(TEntity entity) where TEntity : class;
    }
}
