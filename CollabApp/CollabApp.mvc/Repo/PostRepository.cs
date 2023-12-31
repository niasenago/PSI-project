
using CollabApp.mvc.Context;
using CollabApp.mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace CollabApp.mvc.Repo
{
    public interface IPostRepository : IGenericRepository<Post>
    {
    }
    public class PostRepository : GenericRepository<Post>,  IPostRepository
    {
        public PostRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        //GetAllAsync, GetAsync,DeleteEntity, DeleteEntitiesByExpression  are implemented in GenericRepository

        public override async Task<bool> AddEntity(Post entity)
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


        public override async Task<bool> UpdateEntity(Post entity)
        {
            try
            {
                var existData = await DbSet.FirstOrDefaultAsync(item => item.Id == entity.Id);
                if(existData != null)
                {
                    existData.Id = entity.Id;
                    existData.Title = entity.Title;
                    existData.Description = entity.Description;
                    existData.Author = entity.Author;
                    existData.BoardId = entity.BoardId;
                    existData.MediaFiles = entity.MediaFiles;
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