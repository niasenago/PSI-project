
using CollabApp.API.Context;
using CollabApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CollabApp.API.Repo
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetUserByUsernameAsync(string username);
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
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await DbSet.FirstOrDefaultAsync(u => u.Username == username);
        }


        public override async Task<bool> UpdateEntity(User entity)
        {
            try
            {
                //TODO deep cloning methods (https://stackoverflow.com/questions/129389/how-do-you-do-a-deep-copy-of-an-object-in-net)
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
    }
}