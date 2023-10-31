using CollabApp.mvc.Context;
using CollabApp.mvc.Models;
using CollabApp.mvc.Services;

using CollabApp.mvc.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollabApp.mvc.Controllers
{
    public class PostController : Controller
    {

        private readonly IDBAccess<Post> _db;
        private readonly PostFilterService _postFilterService;
        
        private readonly ApplicationDbContext _context;        
        public PostController(ApplicationDbContext context, PostFilterService postFilterService)
        {
            _context = context;
            _postFilterService = postFilterService;
        }
        public IActionResult Posts()
        {
            var posts = _context.Posts.ToList();
            return View(posts);
        }
        public IActionResult PostView(int Id)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == Id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new Post());
        }

        
        [HttpPost]
        public async Task<IActionResult> Index(Post post)
        {
            ValidationError error = post.Title.IsValidTitle();
            if (error.HasError())
            {
                ViewBag.ErrorMessage = error.ErrorMessage;
                return View();
            }

            error = post.Description.IsValidDescription();
            if (error.HasError())
            {
                ViewBag.ErrorMessage = error.ErrorMessage;
                return View();
            }

            // Convert the DateTime to UTC
            post.DatePosted = post.DatePosted.ToUniversalTime();

            post.Description = ProfanityHandler.CensorProfanities(post.Description);

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

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
        public async Task<IActionResult> AddComment(int Id, string Author, string commentDescription)
        {
            // Retrieve the post from the database
            var post = await _context.Posts.FindAsync(Id);

            if (post == null)
            {
                return NotFound(); // Handle the case where the post doesn't exist
            }

            ValidationError error = commentDescription.IsValidDescription();
            if (error.HasError())
            {
                ViewBag.ErrorMessage = error.ErrorMessage;
                return View("PostView", post);
            }

            commentDescription = ProfanityHandler.CensorProfanities(commentDescription);

            Comment comment = new Comment(Author, commentDescription);

            // Set the DatePosted property to the current UTC time
            comment.DatePosted = DateTime.UtcNow;

            post.Comments.Add(comment);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction("PostView", new { Id });
        }


        
        [HttpPost]
        public IActionResult FilterPosts(string searchTerm = "", string authorName = "", DateTime from = default, DateTime to = default)
        {   
            var filteredPosts = _postFilterService.FilterPosts(searchTerm, authorName, from, to);
            return View("Posts", filteredPosts);
        }
        [HttpPost]
        public IActionResult SortPosts(SortingOption sortBy)
        {
           
            var allPosts =  _context.Posts.ToList();
            var sortedPosts = allPosts;

            switch(sortBy)
            {
                case SortingOption.DescComments:
                    sortedPosts.Sort(new CompareOnlyCommentAmount());
                    sortedPosts.Reverse();
                    break;
                case SortingOption.AscComments:
                    sortedPosts.Sort(new CompareOnlyCommentAmount());
                    break;
                case SortingOption.DescDate:
                    sortedPosts.Sort(new CompareOnlyDates());
                    sortedPosts.Reverse();
                    break;
                case SortingOption.AscDate:
                    sortedPosts.Sort(new CompareOnlyDates());
                    break;
            }
            return View("Posts", sortedPosts);
        }
    }
}
