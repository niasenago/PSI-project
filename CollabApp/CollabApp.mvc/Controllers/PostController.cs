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
        private readonly NotificationService _notificationService;

        public event EventHandler<PostEventArgs>? NewPostAdded;

        public PostController(IDBAccess<Post> db, PostFilterService PostFilterService, NotificationService notificationService)
        {
            _db = db;
            _postFilterService = PostFilterService;
            _notificationService = notificationService;
            _notificationService.SubscribeToNewPostEvent(this);
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

            _db.AddItem(post);

            OnNewPostAdded(new PostEventArgs(post));

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
        } */
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

            ValidationError error = commentDescription.IsValidDescription();
            if (error.HasError())
            {
                ViewBag.ErrorMessage = error.ErrorMessage;
                return View("PostView", post);
            }

            commentDescription = ProfanityHandler.CensorProfanities(commentDescription);

            Comment comment = new Comment(Author, commentDescription);
            post.Comments.Add(comment);
            _db.UpdateItemById(Id, post);
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
            var allPosts = _db.GetAllItems();
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

        protected virtual void OnNewPostAdded(PostEventArgs e)
        {
            NewPostAdded?.Invoke(this, e);
        }

    }

    public class PostEventArgs : EventArgs
    {
        public Post AddedPost { get; }

        public PostEventArgs(Post post)
        {
            AddedPost = post;
        }
    }
}
