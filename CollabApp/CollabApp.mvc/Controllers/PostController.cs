using System.Linq.Expressions;
using CollabApp.mvc.Context;
using CollabApp.mvc.Models;
using CollabApp.mvc.Services;
using CollabApp.mvc.Validation;
using CollabApp.mvc.Exceptions;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollabApp.mvc.Controllers
{
    public class PostController : Controller
    {

        private readonly PostFilterService _postFilterService;
        private readonly ICloudStorageService _cloudStorageService;
        private readonly ApplicationDbContext _context;        
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NotificationService _notificationService;
        
        public event EventHandler<Post>? NewPostAdded;

        public PostController(ApplicationDbContext context, PostFilterService postFilterService, IHttpContextAccessor httpContextAccessor, ICloudStorageService cloudStorageService, NotificationService notificationService)
        {
            _context = context;
            _postFilterService = postFilterService;
            _httpContextAccessor = httpContextAccessor;
            _cloudStorageService = cloudStorageService;
            _notificationService = notificationService;
            _notificationService.SubscribeToNewPostEvent(this);
        }

        public IActionResult Posts()
        {
            var posts = _context.Posts.ToList();
            return View(posts);
        }
        public async Task<IActionResult> PostViewAsync(int Id)
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

            await GenerateSignedUrl(post);   
                     

            return View(post);
        }

        private async Task GenerateSignedUrl(Post post)
        {
            //Get signed URL only when Saved file Name is available
            if(!string.IsNullOrEmpty(post.SavedFileName))
            {
                post.SignedUrl = await _cloudStorageService.GetSignedUrlAsync(post.SavedFileName);
                post.fileType = await _cloudStorageService.GetFileType(post.SavedFileName);
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new Post());
        }


        [HttpPost]
        public async Task<IActionResult> Index([Bind("Author, Title, Description, Photo, SavedUrl, SavedFileName")]  Post post) //add post
        {
            try {
                UserValidator.UserExists(_context, post.Author.Id);
                post.Title.IsValidTitle();
                post.Description.IsValidDescription();
            }
            catch(ValidationException err)
            {
                ViewBag.ErrorMessage = err.Message;
                return View();
            }
            catch(InvalidUserException err)
            {
                ViewBag.ErrorMessage = err.Message;
                return RedirectToAction("Login", "Login");
            }

            if(post.Photo != null)
            {
                post.SavedFileName = GenerateFileNameToSave(post.Photo.FileName);
                post.SavedUrl = await _cloudStorageService.UploadFileAsync(post.Photo, post.SavedFileName);
            }

            post.Description = ProfanityHandler.CensorProfanities(post.Description);

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            
            OnNewPostAdded(post);
            
            return RedirectToAction("Posts");
        }

        private string? GenerateFileNameToSave(string incomingFileName)
        {
            var fileName = Path.GetFileNameWithoutExtension(incomingFileName);
            var extension = Path.GetExtension(incomingFileName);
            return $"{fileName}-{DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss")}{extension}";
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int Id, int AuthorId, string commentDescription)
        {
            var post = await _context.Posts.FindAsync(Id);

            if (post == null)
            {
                // Handle the case where the post with the given ID is not found.
                return NotFound();
            }

            try {
                commentDescription.IsValidDescription();
                UserValidator.UserExists(_context, AuthorId);
            }
            catch(ValidationException err) 
            {
                ViewBag.ErrorMessage = err.Message;
                return RedirectToAction("PostView", post);    
            }
            catch(InvalidUserException err)
            {
                ViewBag.ErrorMessage = err.Message;
                return RedirectToAction("Login", "Login");
            }

            commentDescription = ProfanityHandler.CensorProfanities(commentDescription);
            var comment = new Comment(AuthorId, commentDescription, Id);
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

            switch (sortBy)
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

            if (currentUser != post.Author.Username)
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

            if (currentUser != existingPost.Author.Username)
            {
                TempData["ErrorMessage"] = "You are not authorized to edit this post.";
                return RedirectToAction("PostView", new { id });
            }

            try {
                updatedPost.Title.IsValidTitle();
            }
            catch(ValidationException err) 
            {
                ViewBag.ErrorMessage = err.Message;
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

            if (currentUser != post.Author.Username)
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

        protected virtual void OnNewPostAdded(Post post)
        {
            NewPostAdded?.Invoke(this, post);
        }
    }
}
