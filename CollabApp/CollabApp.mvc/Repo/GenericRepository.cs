using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollabApp.mvc.Context;
using Microsoft.EntityFrameworkCore;

namespace CollabApp.mvc.Repo
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected ApplicationDbContext dbContext;
        internal DbSet<T> DbSet {get; set; }

        public GenericRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.DbSet = this.dbContext.Set<T>();
        }
        public virtual Task<List<T>> GetAllAsync()
        {
            return this.DbSet.ToListAsync();
        }

        public virtual Task<T> GetAsync(int id)
        {
            throw new NotImplementedException();
        }
        public virtual Task<bool> AddEntity(T entity)
        {
            throw new NotImplementedException();
        }
        public virtual Task<bool> DeleteEntity(int id)
        {
            throw new NotImplementedException();
        }



        public virtual Task<bool> UpdateEntity(T entity)
        {
            throw new NotImplementedException();
        }
    }
}