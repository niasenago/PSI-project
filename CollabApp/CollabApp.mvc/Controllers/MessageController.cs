using CollabApp.mvc.Models;
using CollabApp.mvc.Services;
using CollabApp.mvc.Context;
using Microsoft.AspNetCore.Mvc;

namespace CollabApp.mvc.Controllers
{
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _db;

        public MessageController(ApplicationDbContext db)
        {
            _db = db;
        }
        [Route("MessageController/Messages")]
        public IActionResult Messages()
        {
            var messages = _db.Messages.ToList();
            return Json(messages);
        }
        public IActionResult MessageView(int Id)
        {
            return View(_db.Messages.FirstOrDefault(p => p.Id == Id));
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View(new Message());
        }
        [HttpPost]
        public async Task<IActionResult> Index(Message message)
        {
            _db.Messages.Add(message);
            await _db.SaveChangesAsync();
            return RedirectToAction("Messages");
        }
        public void AddMessage(Message message)
        {
            _db.Messages.Add(message);
            _db.SaveChangesAsync();
        }
        public List<Message> GetAllMessages()
        {
            return _db.Messages.ToList();
        }
        public Message GetMessageById(int Id)
        {
            return _db.Messages.FirstOrDefault(p => p.Id == Id); ;
        }
    }
}
