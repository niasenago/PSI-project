
using CollabApp.mvc.Context;
using CollabApp.mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace CollabApp.mvc.Repo
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> IsUsernameTakenAsync(string username);
    }
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        //GetAllAsync, GetAsync,DeleteEntity, DeleteEntitiesByExpression  are implemented in GenericRepository

        public override async Task<bool> AddEntity(User entity)
        {
            try 
            {
                await DbSet.AddAsync(entity);
                return true;
            }
            catch (Exception e)
            {
                throw;
            }
        }


        public override async Task<bool> UpdateEntity(User entity)
        {
            try
            {
                var existData = await DbSet.FirstOrDefaultAsync(item => item.Id == entity.Id);
                if(existData != null)
                {
                    existData.Id = entity.Id;
                    existData.Username = entity.Username;
                    
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await DbSet.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> IsUsernameTakenAsync(string username)
        {
            return await DbSet.AnyAsync(u => u.Username == username);
        }
    }
}