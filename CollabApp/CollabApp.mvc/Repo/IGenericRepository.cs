using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollabApp.mvc.Models;

namespace CollabApp.mvc.Repo
{
    public interface IGenericRepository<T> where T : class, IBaseEntity
    {
        Task <List<T>> GetAllAsync();
        Task <T> GetAsync(int id);
        Task <bool> AddEntity(T entity);
        Task <bool> UpdateEntity(T entity);
    }
}