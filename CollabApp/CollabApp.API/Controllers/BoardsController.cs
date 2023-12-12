using CollabApp.mvc.Exceptions;
using CollabApp.mvc.Models;
using CollabApp.SharedObjects.Dto;

using CollabApp.mvc.Repo;
using CollabApp.mvc.Validation;
using Microsoft.AspNetCore.Mvc;

namespace CollabApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardsController : ControllerBase
    {
        private readonly ILogger<BoardsController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public BoardsController(ILogger<BoardsController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Board>>> GetBoards()
        {
            Console.WriteLine("API: GetBoards works");
            var boards = await _unitOfWork.BoardRepository.GetAllAsync();
            return Ok(boards);
        }

        [HttpPost]
        public async Task<ActionResult<Board>> CreateBoard([FromBody] CreateBoardDto createBoardDto)
        {
            Console.WriteLine("API: CreateBoard works");
            try
            {
                createBoardDto.BoardName.IsValidTitle();
            }
            catch (ValidationException err)
            {
                return BadRequest(err.Message);
            }

            var board = new Board { BoardName = createBoardDto.BoardName, BoardDescription = createBoardDto.BoardDescription };
            bool isAdded = await _unitOfWork.BoardRepository.AddEntity(board);
            await _unitOfWork.CompleteAsync();

            if (isAdded)
            {
                Board createdBoard = await _unitOfWork.BoardRepository.GetAsync(board.Id);

                return CreatedAtAction(nameof(GetBoard), new { id = createdBoard.Id }, createdBoard);
            }
            else
            {
                return BadRequest("Board could not be created");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Board>> GetBoard(int id)
        {
            Console.WriteLine("API: GetBoard works");
            var board = await _unitOfWork.BoardRepository.GetAsync(id);
            if (board == null) return NotFound();
            return Ok(board);
        }
    }
}