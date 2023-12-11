
using System.Linq.Expressions;
using CollabApp.API.Models;

namespace CollabApp.API.Repo
{
    public interface IGenericRepository<T> where T : class, IBaseEntity
    {
        Task <List<T>> GetAllAsync();
        Task <T> GetAsync(int id);
        Task <bool> AddEntity(T entity);
        Task <bool> UpdateEntity(T entity);
        Task <bool> DeleteEntity(T entity);
        Task<bool> DeleteEntitiesByExpression(Expression<Func<T, bool>> predicate);

    }
}