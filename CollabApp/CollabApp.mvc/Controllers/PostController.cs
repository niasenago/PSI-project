using CollabApp.mvc.Data;
using CollabApp.mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollabApp.mvc.Controllers
{
    public class PostController : Controller
    {

        private readonly IDBAccess<Post> _db;

        public PostController(IDBAccess<Post> db)
        {
            _db = db;
        }
        public IActionResult Posts()
        {
            return View(_db.GetAllItems());
        }
        public IActionResult PostView(int Id)
        {
            return View(_db.GetItemById(Id));
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View(new Post());
        }
        [HttpPost]
        public async Task<IActionResult> Index(Post post)
        {
            _db.AddItem(post);
            return RedirectToAction("Posts");
        }
        public void AddPost(Post post)
        {
            _db.AddItem(post);
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
            return _db.GetAllItems();
        }
        public Post GetPostById(int Id)
        {
            return _db.GetItemById(Id);
        }
        [HttpPost]
        public IActionResult AddComment(int Id, string Author, string commentDescription)
        {
            Console.WriteLine(Id);
            List<Post> posts = _db.GetAllItems();
            Post post = posts.FirstOrDefault(p => p.Id == Id);

            Comment comment = new Comment(Author, commentDescription);
            post.Comments.Add(comment);
            _db.AddItem(post);

            //return PostView(Id);
            return RedirectToAction("PostView", new {Id});
        }
    }
}
