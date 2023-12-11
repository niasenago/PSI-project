
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CollabApp.mvc.Models;
using CollabApp.mvc.Context;
using CollabApp.mvc.Repo;
using CollabApp.mvc.Validation;
using CollabApp.mvc.Exceptions;
using CollabApp.mvc.Dto;
using Newtonsoft.Json;
using CollabApp.mvc.Services;

namespace CollabApp.mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly IHttpServiceClient _httpServiceClient;// Reusable HttpClient for API requests
    //private readonly HttpClient _apiClient; 

    public HomeController(ILogger<HomeController> logger, IHttpServiceClient httpServiceClient)
    {
        _logger = logger;
        _httpServiceClient = httpServiceClient;
        //_httpClientFactory = httpClientFactory;
        //_apiClient = httpClientFactory.CreateClient("Api");
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            // Make a GET request to the API endpoint to get the boards
            var boardsJson = await _httpServiceClient.GetAsync("api/Boards");

            // Deserialize the JSON string into a list of Board objects
            var boards = JsonConvert.DeserializeObject<List<Board>>(boardsJson);

            return View(boards);
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError(ex, "Error retrieving boards.");

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
        try
        {
            board.BoardName.IsValidTitle();
            var createBoardDtoJson = JsonConvert.SerializeObject(new CreateBoardDto { BoardName = board.BoardName });
            var responseContent = await _httpServiceClient.PostAsync("api/Boards", createBoardDtoJson);


        }
        catch (ValidationException err)
        {
            ViewBag.ErrorMessage = err.Message;
            return View();
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Error creating board. Please try again.";
            return View();
        }

        return RedirectToAction("Index"); // Redirect to the appropriate action after successful creation
    }
}
