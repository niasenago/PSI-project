using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollabApp.mvc.Context;
using CollabApp.mvc.Repo;

namespace CollabApp.mvc.Repo
{
    //idk do we need IDisposable
    public class UnitOfWork : IUnitOfWork
    {
        public IPostRepository postRepository {get; private set;}
        public IBoardRepository boardRepository {get; private set;}
        public ICommentRepository commentRepository {get; private set;}
        private readonly ApplicationDbContext dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            postRepository = new PostRepository(dbContext);
            boardRepository = new BoardRepository(dbContext);
            commentRepository = new CommentRepository(dbContext);            
        }
        public UnitOfWork(IPostRepository postRepository, IBoardRepository boardRepository,ICommentRepository commentRepository, ApplicationDbContext dbContext)
        {
            this.postRepository = postRepository;
            this.boardRepository = boardRepository;
            this.commentRepository = commentRepository;
            this.dbContext = dbContext;
        }

        public async Task CompleteAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}