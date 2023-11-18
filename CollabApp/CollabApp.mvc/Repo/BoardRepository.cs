using CollabApp.mvc.Context;
using CollabApp.mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace CollabApp.mvc.Repo
{
    public interface IBoardRepository : IGenericRepository<Board>
    {
    }
    public class BoardRepository : GenericRepository<Board>, IBoardRepository
    {
        public BoardRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        // GetAllAsync GetAsync are implemented in GenericRepository
        public override async Task<bool> AddEntity(Board entity)
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
        public override async Task<bool> UpdateEntity(Board entity)
        {
            try
            {
                //TODO deep cloning methods (https://stackoverflow.com/questions/129389/how-do-you-do-a-deep-copy-of-an-object-in-net)
                var existData = await this.DbSet.FindAsync(entity.BoardId).AsTask();
                if(existData != null)
                {
                    existData.BoardId = entity.BoardId;
                    existData.BoardName = entity.BoardName;
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