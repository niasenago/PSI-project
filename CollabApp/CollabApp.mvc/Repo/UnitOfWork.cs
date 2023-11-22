
using CollabApp.mvc.Context;

namespace CollabApp.mvc.Repo
{
    //idk do we need IDisposable
    public class UnitOfWork : IUnitOfWork
    {
        public IPostRepository PostRepository {get; private set;}
        public IBoardRepository BoardRepository {get; private set;}
        public ICommentRepository CommentRepository {get; private set;}
        public IAttachmentRepository AttachmentRepository {get; private set;}
        public IUserRepository UserRepository {get; private set;}
        private readonly ApplicationDbContext dbContext;


        public UnitOfWork(IPostRepository postRepo, IBoardRepository boardRepo, ICommentRepository commentRepo, IAttachmentRepository attachmentRepo, IUserRepository userRepo, ApplicationDbContext dbContext)
        {
            this.PostRepository = postRepo;
            this.BoardRepository = boardRepo;
            this.CommentRepository = commentRepo;
            this.AttachmentRepository = attachmentRepo;
            this.UserRepository = userRepo;
            this.dbContext = dbContext;
        }

        public async Task CompleteAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}