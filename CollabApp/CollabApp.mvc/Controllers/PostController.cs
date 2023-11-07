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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostController(ApplicationDbContext context, PostFilterService postFilterService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _postFilterService = postFilterService;
            _httpContextAccessor = httpContextAccessor;
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
            var comments = new List<Comment>();
            comments = _context.Comments
                .Where(item => item.PostId == Id)
                .ToList();

            ViewData["Comments"] = comments;

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
                return RedirectToAction("PostView", post);
            }
            commentDescription = ProfanityHandler.CensorProfanities(commentDescription);
            var comment = new Comment(Author, commentDescription, Id);
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

        [HttpPost]
        public async Task<IActionResult> ChangeRating(int postId, int commentId, RatingOption rating)
        {
            var post = await _context.Posts.FindAsync(postId);
            if(null == post)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(commentId);
            if(null == comment)
            {
                return NotFound();
            }

            comment.Rating += (rating == RatingOption.Upvote) ? 1 : -1;
            await _context.SaveChangesAsync();

            return RedirectToAction("PostView", post);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            var currentUser = _httpContextAccessor.HttpContext.Session.GetString("Username");

            if (currentUser != post.Author)
            {
                TempData["ErrorMessage"] = "You are not authorized to edit this post.";
                return RedirectToAction("PostView", new { id });
            }

            return View(post);
        }

        [HttpPost]
        public IActionResult Edit(int id, Post updatedPost)
        {
            var existingPost = _context.Posts.FirstOrDefault(p => p.Id == id);
            if (existingPost == null)
            {
                return NotFound();
            }

            var currentUser = _httpContextAccessor.HttpContext.Session.GetString("Username");

            if (currentUser != existingPost.Author)
            {
                TempData["ErrorMessage"] = "You are not authorized to edit this post.";
                return RedirectToAction("PostView", new { id });
            }

            // Update the post properties with the changes

            ValidationError error = updatedPost.Title.IsValidTitle();
            if (error.HasError())
            {
                Console.WriteLine(error.ErrorMessage);
                ViewBag.ErrorMessage = error.ErrorMessage;
                return View("Edit", existingPost);
            }

            existingPost.Title = updatedPost.Title;
            existingPost.Description = updatedPost.Description;
            _context.SaveChanges();

            return RedirectToAction("PostView", new { id });
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            var currentUser = _httpContextAccessor.HttpContext.Session.GetString("Username");

            if (currentUser != post.Author)
            {
                TempData["ErrorMessage"] = "You are not authorized to delete this post.";
                return RedirectToAction("PostView", new { id });
            }

            //remove all comments associated with the post before deletion
            _context.Comments.RemoveRange(_context.Comments.Where(c => c.PostId == id));
            _context.Posts.Remove(post);
            _context.SaveChanges();

            return RedirectToAction("Posts");
        }

    }
}
