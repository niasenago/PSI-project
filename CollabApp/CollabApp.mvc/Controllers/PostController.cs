using CollabApp.mvc.Repo;

using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using CollabApp.mvc.Models;
using CollabApp.mvc.Services;
using CollabApp.mvc.Validation;
using CollabApp.mvc.Exceptions;
using CollabApp.mvc.Delegates;
using Microsoft.AspNetCore.Mvc;

[assembly: InternalsVisibleTo("CollabApp.UnitTests")]
namespace CollabApp.mvc.Controllers
{
    public class PostController : Controller
    {

        private readonly PostFilterService _postFilterService;
        private readonly ICloudStorageService _cloudStorageService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        
        // public event EventHandler<Post>? NewPostAdded;
        public event NewPostAddedEventHandler NewPostAdded; 

        public PostController( PostFilterService postFilterService, IHttpContextAccessor httpContextAccessor, ICloudStorageService cloudStorageService, NotificationService notificationService, IUnitOfWork unitOfWork)
        {
            _postFilterService = postFilterService;
            _httpContextAccessor = httpContextAccessor;
            _cloudStorageService = cloudStorageService;
            _notificationService = notificationService;
            _notificationService.SubscribeToNewPostEvent(this);
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> PostsAsync(int? boardId) //get boardId from route
        {
            if (boardId == null || boardId == 0)
            {
                //!CHANGE THIS
                boardId = 0;
                ViewData["BoardId"] = boardId;
                // Handle the case when no board is selected
                //return RedirectToAction("Index");
                var posts = await _unitOfWork.PostRepository.GetAllAsync();
                return View(posts);
            }
            
            else
            {
                var posts = await _unitOfWork.PostRepository.GetAllAsync();
                posts = posts
                     .Where(p => p.BoardId == boardId)
                     .ToList();
                ViewData["BoardId"] = boardId;
                return View(posts);
            }
        }

        public async Task<IActionResult> PostViewAsync(int Id)
        {
            //var post = _context.Posts.FirstOrDefault(p => p.Id == Id);
            var post = await _unitOfWork.PostRepository.GetAsync(Id);
            if (post == null)
            {
                return NotFound();
            }
            var comments = new List<Comment>();
            comments = await _unitOfWork.CommentRepository.GetAllAsync();

            comments = comments
                .Where(item => item.PostId == Id)
                .ToList();

            ViewData["Comments"] = comments;
            ViewData["Attachments"] = await GetFilesFromDatabase(post.Id);

            return View(post);
        }

        internal async Task<List<Attachment>> GetFilesFromDatabase(int Id)
        {
            var files = await _unitOfWork.AttachmentRepository.GetAllAsync();

            files = files
                .Where(item => item.PostId == Id)
                .ToList();

            var urlFetchTasks = files.Select(async file =>
            {
                //Get signed URL only when Saved file Name is available
                if(!string.IsNullOrEmpty(file.FileName))
                {
                    file.SignedUrl = await _cloudStorageService.GetSignedUrlAsync(file.FileName);
                    file.FileType = await _cloudStorageService.GetFileType(file.FileName);
                }
            });

            await Task.WhenAll(urlFetchTasks);

            return files;
        }

        internal async Task AddFilesToDatabase(Post post)
        {
            if(post.MediaFiles != null && post.MediaFiles.Count > 0)
            {
                var uploadTasks = post.MediaFiles.Select(async MediaFile =>
                {
                    string SavedFileName = GenerateFileNameToSave(MediaFile.FileName);
                    string SavedUrl = await _cloudStorageService.UploadFileAsync(MediaFile, SavedFileName);
            
                    var file = new Attachment(SavedFileName, SavedUrl, post.Id);
                    await _unitOfWork.AttachmentRepository.AddEntity(file);
                });

                await Task.WhenAll(uploadTasks);
            }
        }
        /*
        The DisplayForm method is used for displaying the form to create a new post
        */

        [HttpPost]
        public IActionResult DisplayForm(int boardId)
        {
            var post = new Post();
            if (boardId == null)
            {
                //!CHANGE THIS
                boardId = 0;
                Console.WriteLine("In DisplayForm method boardId value null");
                // Handle the case when no board is selected
                // return RedirectToAction("Index");
            }
            post.BoardId = boardId;
            Console.WriteLine("In DisplayForm method boardId value" + post.BoardId);
            return View("Index", post); // Return the Index view with the Post model
        }
            
        /*
        The Index method (POST) is used for handling the submission of the form, 
        processing the form data, and creating a new post.
        */

        [HttpPost]
        public async Task<IActionResult> Index([Bind("AuthorId, BoardId, Title, Description, MediaFiles")]  Post post) //add post
        {
            try {
                //UserValidator.UserExists(_context, post.AuthorId); TODO: change this
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

            await AddFilesToDatabase(post);
            Console.WriteLine(post.AuthorId + " boardID " + post.BoardId + " postID " + post.Id);

            post.Description = ProfanityHandler.CensorProfanities(post.Description);

            var data = await _unitOfWork.PostRepository.AddEntity(post);
            await _unitOfWork.CompleteAsync();
            
            OnNewPostAdded(post);
            
            return RedirectToAction("Posts", new { boardId = post.BoardId});
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
            var post = await _unitOfWork.PostRepository.GetAsync(Id);

            if (post == null)
            {
                // Handle the case where the post with the given ID is not found.
                return NotFound();
            }

            try {
                commentDescription.IsValidDescription();
                UserValidator.UserExists(_unitOfWork, AuthorId); //TODO: change this
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

            var data = await _unitOfWork.CommentRepository.AddEntity(comment);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction("PostView", new { id = Id }); // Redirect to the post view page.

        }
        
        [HttpPost]
        public IActionResult FilterPosts(string searchTerm = "", string authorName = "", DateTime from = default, DateTime to = default, int boardId = 0)
        {
            ViewData["BoardId"] = boardId;
            var filteredPosts = _postFilterService.FilterPosts(searchTerm, authorName, from, to, boardId);
            return View("Posts", filteredPosts);
        }
        [HttpPost]
        public async Task<IActionResult> SortPosts(int boardId, SortingOption sortBy)
        {
            ViewData["BoardId"] = boardId;           
            var allPosts = await _unitOfWork.PostRepository.GetAllAsync();
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
            var post = await _unitOfWork.PostRepository.GetAsync(postId);
            if(null == post)
            {
                return NotFound();
            }
            var comment = await _unitOfWork.CommentRepository.GetAsync(commentId);
            if(null == comment)
            {
                return NotFound();
            }

            comment.Rating += (rating == RatingOption.Upvote) ? 1 : -1;
            await _unitOfWork.CompleteAsync();
            return RedirectToAction("PostView", post);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _unitOfWork.PostRepository.GetAsync(id);
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
        public async Task<IActionResult> Edit(int id, Post updatedPost)
        {
            //var existingPost = _context.Posts.FirstOrDefault(p => p.Id == id);
            var existingPost = await _unitOfWork.PostRepository.GetAsync(id);
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

            //_context.SaveChanges();
            await _unitOfWork.CompleteAsync();


            return RedirectToAction("PostView", new { id });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var post = await _unitOfWork.PostRepository.GetAsync(id);
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

            await _unitOfWork.CommentRepository.DeleteEntitiesByExpression(c => c.PostId == id);
            await _unitOfWork.AttachmentRepository.DeleteEntitiesByExpression(c => c.PostId == id);
            await _unitOfWork.PostRepository.DeleteEntity(post);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction("Posts");
        }

        protected virtual void OnNewPostAdded(Post post)
        {
            NewPostAdded?.Invoke(this, post);
        }
    }
}
