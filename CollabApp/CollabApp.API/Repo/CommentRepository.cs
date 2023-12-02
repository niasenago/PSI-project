
using CollabApp.mvc.Models;
using CollabApp.mvc.Context;


namespace CollabApp.mvc.Repo
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
    }
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        //GetAllAsync, GetAsync,DeleteEntity, DeleteEntitiesByExpression  are implemented in GenericRepository
        public CommentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public override async Task<bool> AddEntity(Comment entity)
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
        public override async Task<bool> UpdateEntity(Comment entity)
        {
            try
            {
                //TODO deep cloning methods (https://stackoverflow.com/questions/129389/how-do-you-do-a-deep-copy-of-an-object-in-net)
                var existData = await this.DbSet.FindAsync(entity.Id).AsTask();
                if(existData != null)
                {
                    existData.Id = entity.Id;
                    existData.PostId = entity.PostId;
                    existData.Description = entity.Description;
                    existData.Author = entity.Author;
                    existData.PostId = entity.PostId;
                    existData.Rating = entity.Rating;
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