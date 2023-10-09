using CollabApp.mvc.Data;
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

        public PostController(IDBAccess<Post> db, PostFilterService PostFilterService)
        {
            _db = db;
            _postFilterService = PostFilterService; // Ensure the casing is correct here.
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
            if(post.Title == null || post.Title.IsValidTitle() != ValidationResult.Valid)
                return View();

            if(post.Description != null && post.Description.IsValidDescription() != ValidationResult.Valid)
                return View();
                
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
            Post post = _db.GetItemById(Id);

            if(commentDescription == null || commentDescription.IsValidDescription() != ValidationResult.Valid)
                return View("PostView", post);

            Comment comment = new Comment(Author, commentDescription);
            post.Comments.Add(comment);
            _db.UpdateItemById(Id, post);
            return RedirectToAction("PostView", new {Id});
        }
        [HttpPost]
        public IActionResult FilterPosts(string searchTerm = "", string authorName = "", DateTime from = default, DateTime to = default)
        {   
            var filteredPosts = _postFilterService.FilterPosts(searchTerm, authorName, from, to);
            return View("Posts", filteredPosts);
        }
        [HttpPost]
        public IActionResult SortPosts(string sortBy)
        {
            var allPosts = _db.GetAllItems();
            var sortedPosts = allPosts;

            if(sortBy == "desc-comments") 
            {
                sortedPosts.Sort(new CompareOnlyCommentAmount());
                sortedPosts.Reverse();
            }
            else if(sortBy == "asc-comments")
            {
                sortedPosts.Sort(new CompareOnlyCommentAmount());
            }
            else if(sortBy == "desc-date")
            {
                sortedPosts.Sort(new CompareOnlyDates());
                sortedPosts.Reverse();
            }
            else if(sortBy == "asc-date")
            {
                sortedPosts.Sort(new CompareOnlyDates());
            }
            return View("Posts", sortedPosts);
        }
    }
}
