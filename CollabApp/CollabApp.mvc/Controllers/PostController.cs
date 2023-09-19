using CollabApp.mvc.Data;
using CollabApp.mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace CollabApp.mvc.Controllers
{
    public class PostController : Controller
    {

        private readonly ApplicationDbContext _db;

        public PostController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Posts()
        {
            return View(GetAllPosts());
        }
        public IActionResult PostView(int Id)
        {
            return View(GetPostById(Id));
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View(new Post());
        }
        [HttpPost]
        public async Task<IActionResult> Index(Post post)
        {
            AddPost(post);
            if (await AddPostAsync())
                return RedirectToAction("Posts");
            else
                return View();
        }
        public void AddPost(Post post)
        {
            _db.Posts.Add(post);
        }
        public async Task<bool> AddPostAsync()
        {
            if(await _db.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }
        public List<Post> GetAllPosts()
        {
            return _db.Posts.ToList();
        }
        public Post GetPostById(int Id)
        {
            return _db.Posts.FirstOrDefault(p => p.Id == Id);
        }
    }
}
