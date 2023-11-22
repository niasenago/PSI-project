
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CollabApp.mvc.Models;
using CollabApp.mvc.Context;
using CollabApp.mvc.Repo;
using CollabApp.mvc.Validation;
using CollabApp.mvc.Exceptions;

namespace CollabApp.mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var boards = await _unitOfWork.BoardRepository.GetAllAsync();
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
    public async Task<IActionResult> CreateBoard(Board board)
    {
        try {
            board.BoardName.IsValidTitle();
        }
        catch(ValidationException err)
        {
            ViewBag.ErrorMessage = err.Message;
            return View();
        }

        var data = await _unitOfWork.BoardRepository.AddEntity(board);
        await _unitOfWork.CompleteAsync();
        
        return RedirectToAction("Index"); // Redirect to the appropriate action after successful creation
    }
}
