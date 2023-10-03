using CollabApp.mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace CollabApp.mvc.Controllers
{
    public class MessageController : Controller
    {
        private readonly IDBAccess<Message> _db;

        public MessageController(IDBAccess<Message> db)
        {
            _db = db;
        }
        [Route("MessageController/Messages")]
        public IActionResult Messages()
        {
            var messages = _db.GetAllItems();
            return Json(messages);
        }
        public IActionResult MessageView(int Id)
        {
            return View(_db.GetItemById(Id));
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View(new Message());
        }
        [HttpPost]
        public async Task<IActionResult> Index(Message message)
        {
            _db.AddItem(message);
            return RedirectToAction("Messages");
        }
        public void AddMessage(Message message)
        {
            _db.AddItem(message);
        }
        public List<Message> GetAllMessages()
        {
            return _db.GetAllItems();
        }
        public Message GetMessageById(int Id)
        {
            return _db.GetItemById(Id);
        }
    }
}
