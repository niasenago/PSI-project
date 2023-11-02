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
        public async Task<IActionResult> Index(Post post) //add post
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


            post.Description = ProfanityHandler.CensorProfanities(post.Description);

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Posts");
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int Id, string Author, string commentDescription)
        {
            var post = await _context.Posts.FindAsync(Id);

            if (post == null)
            {
                // Handle the case where the post with the given ID is not found.
                return NotFound();
            }

            ValidationError error = commentDescription.IsValidDescription();
            if (error.HasError())
            {
                ViewBag.ErrorMessage = error.ErrorMessage;
                return View("PostView", post);
            }
            commentDescription = ProfanityHandler.CensorProfanities(commentDescription);
            var comment = new Comment(Author, commentDescription);
            comment.PostId = Id;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("PostView", new { id = Id }); // Redirect to the post view page.
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
