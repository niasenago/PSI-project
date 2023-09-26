using CollabApp.mvc.Data;
using CollabApp.mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace CollabApp.mvc.Controllers
{
    public class PostController : Controller
    {

        private readonly IDBAccess _db;

        public PostController(IDBAccess db)
        {
            _db = db;
        }
        public IActionResult Posts()
        {
            return View(_db.GetAllPosts());
        }
        public IActionResult PostView(int Id)
        {
            return View(_db.GetPostById(Id));
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View(new Post());
        }
        [HttpPost]
        public async Task<IActionResult> Index(Post post)
        {
            _db.AddPost(post);
            return RedirectToAction("Posts");
        }
        public void AddPost(Post post)
        {
            _db.AddPost(post);
        }
        /*
        public async Task<bool> AddPostAsync()
        {
            if(await _db.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }*/
        public List<Post> GetAllPosts()
        {
            return _db.GetAllPosts();
        }
        public Post GetPostById(int Id)
        {
            return _db.GetPostById(Id);
        }
    }
}
