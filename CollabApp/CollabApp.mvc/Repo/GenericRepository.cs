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
        //this method can be overriden
        public virtual async Task<List<T>> GetAllAsync() 
        {
            return await this.DbSet.ToListAsync();
        }

        //this method can be overriden
        public virtual async Task<T> GetAsync(int id)
        {
            return await this.DbSet.FindAsync(id).AsTask();
        }
        //this method can be overriden
        public virtual async Task<bool> AddEntity(T entity)
        {
            throw new NotImplementedException();
        }
        //method should be overriden
        public virtual async Task<bool> DeleteEntity(int id)
        {
            throw new NotImplementedException();
        }

        //method should be overriden
        public virtual async Task<bool> UpdateEntity(T entity)
        {
            throw new NotImplementedException();
        }
    }
}