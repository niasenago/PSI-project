using CollabApp.API.Exceptions;
using CollabApp.API.Models;
using CollabApp.API.Dto;

using CollabApp.API.Repo;
using CollabApp.API.Validation;
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
            var boards = await _unitOfWork.BoardRepository.GetAllAsync();
            return Ok(boards);
        }

        [HttpPost]
        public async Task<ActionResult<Board>> CreateBoard([FromBody] CreateBoardDto createBoardDto)
        {
            try
            {
                createBoardDto.BoardName.IsValidTitle();
            }
            catch (ValidationException err)
            {
                return BadRequest(err.Message);
            }

            var board = new Board { BoardName = createBoardDto.BoardName };
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
            var board = await _unitOfWork.BoardRepository.GetAsync(id);
            if (board == null) return NotFound();
            return Ok(board);
        }
    }
}