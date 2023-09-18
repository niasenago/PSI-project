using Microsoft.AspNetCore.Mvc;

namespace CollabApp.mvc.Controllers
{
    public class PostController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
