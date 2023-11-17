using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollabApp.mvc.Repo
{
    public interface IGenericRepository<T> where T : class
    {
        Task <List<T>> GetAllAsync();
        Task <List<T>> GetAsync(int id);
        Task <bool> AddEntity(T entity);
        Task <bool> UpdateEntity(T entity);
    }
}