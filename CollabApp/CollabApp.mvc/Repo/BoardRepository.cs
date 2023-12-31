using CollabApp.mvc.Context;
using CollabApp.mvc.Models;

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
        // GetAllAsync, GetAsync,DeleteEntity, DeleteEntitiesByExpression  are implemented in GenericRepository
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
                var existData = await this.DbSet.FindAsync(entity.Id).AsTask();
                if(existData != null)
                {
                    existData.Id = entity.Id;
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