
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CollabApp.mvc.Models;
using CollabApp.mvc.Context;
using CollabApp.mvc.Repo;
using CollabApp.mvc.Validation;
using CollabApp.mvc.Exceptions;
using CollabApp.mvc.Dto;
using Newtonsoft.Json;

namespace CollabApp.mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _apiClient; // Reusable HttpClient for API requests

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _httpClientFactory = httpClientFactory;
        _apiClient = httpClientFactory.CreateClient("Api");
    }

    public async Task<IActionResult> Index()
    {
        // Make a GET request to the API endpoint to get the boards
        var response = await _apiClient.GetAsync("api/Boards");

        if (response.IsSuccessStatusCode)
        {
            // Read and parse the content of the successful response
            var content = await response.Content.ReadAsStringAsync();
            var boards = JsonConvert.DeserializeObject<List<Board>>(content);

            return View(boards);
        }
        else
        {
            // Handle API error response
            ViewBag.ErrorMessage = "Error retrieving boards. Please try again.";
            return View();
        }
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
            TempData["ErrorMessage"] = err.Message;
            return RedirectToAction("Index");
        }

        var response = await _apiClient.PostAsJsonAsync("api/Boards", new CreateBoardDto { BoardName = board.BoardName });

        if (!response.IsSuccessStatusCode)
        {
            // Handle API error response
            ViewBag.ErrorMessage = "Error creating board. Please try again.";
            return View();
        }
        
        return RedirectToAction("Index"); // Redirect to the appropriate action after successful creation
    }
}
