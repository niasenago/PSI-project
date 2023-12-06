
using System.Linq.Expressions;
using CollabApp.mvc.Context;
using CollabApp.mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace CollabApp.mvc.Repo
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IBaseEntity
    {
        protected ApplicationDbContext dbContext;
        internal DbSet<T> DbSet {get; set; }

        public GenericRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.DbSet = this.dbContext.Set<T>();
        }
        //this method can be overriden
        public virtual async Task<List<T>> GetAllAsync(Expression<Func<T, object>> include = null)
        {
            IQueryable<T> query = DbSet;

            if (include != null)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        //this method can be overriden
        public virtual async Task<T> GetAsync(int id, Expression<Func<T, object>> include = null)
        {
            IQueryable<T> query = DbSet;

            if (include != null)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }
        //this method can be overriden
        public virtual async Task<bool> AddEntity(T entity)
        {
            throw new NotImplementedException();
        }

        //method should be overriden
        public virtual async Task<bool> UpdateEntity(T entity)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> DeleteEntity(T entity)
        {
            try
            {
                var existingEntity = await DbSet.FindAsync(entity.Id);
                
                if (existingEntity != null)
                {
                    DbSet.Remove(existingEntity);
                    await dbContext.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                // Handle exceptions as needed
                throw;
            }
        }
        public virtual async Task<bool> DeleteEntitiesByExpression(Expression<Func<T, bool>> predicate)
        {
            try
            {
                var entitiesToDelete = await DbSet.Where(predicate).ToListAsync();

                if (entitiesToDelete.Any())
                {
                    DbSet.RemoveRange(entitiesToDelete);
                    await dbContext.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                // Handle exceptions as needed
                throw;
            }
        }        
    }
}