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

        public IActionResult Index()
        {
            IEnumerable<Post> PostList = _db.Posts; 
            return View(PostList);
        }
    }
}
