using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CollabApp.mvc.Models;
using CollabApp.mvc.Context;

namespace CollabApp.mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var boards = _context.Boards.ToList();
        return View(boards);
    }

    public IActionResult Chat()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    [HttpPost]
    public IActionResult CreateBoard(Board board)
    {
        if (ModelState.IsValid)
        {
            // Set the creation date before adding to the database
            board.CreationDate = DateTime.UtcNow;

            _context.Boards.Add(board);
            _context.SaveChanges();

            return RedirectToAction("Index"); // Redirect to the appropriate action after successful creation
        }

        // If the model state is not valid, return to the create view with validation errors
        return View("Create", board);
    }
}
