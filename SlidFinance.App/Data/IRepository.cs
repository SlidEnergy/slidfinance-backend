using SlidFinance.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.App
{
    public interface IRepositoryWithAccessCheck<T> : IRepository<T, int> where T : class, IUniqueObject<int>
    {
        Task<List<T>> GetListWithAccessCheck(string userId);
    }

    public interface IRepository<TEntity, T> where TEntity : class, IUniqueObject<T>
    {
        Task<TEntity> GetById(T id);
        Task<List<TEntity>> GetList();
        Task<TEntity> Add(TEntity entity);
        Task<TEntity> Update(TEntity entity);
        Task Delete(TEntity entity);
    }
}
