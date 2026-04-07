using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.API.DTOs;
using TaskManager.API.Models;
using TaskManager.API.Repositories;

namespace TaskManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;

        public TasksController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        private int GetUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                return userId;
            }
            throw new Exception("User not found");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemResponseDto>>> GetTasks()
        {
            var userId = GetUserId();
            var tasks = await _taskRepository.GetTasksByUserIdAsync(userId);
            
            var dtos = tasks.Select(t => new TaskItemResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsCompleted = t.IsCompleted
            });

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItemResponseDto>> GetTask(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            var userId = GetUserId();

            if (task == null || task.UserId != userId)
            {
                return NotFound();
            }

            return Ok(new TaskItemResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted
            });
        }

        [HttpPost]
        public async Task<ActionResult<TaskItemResponseDto>> CreateTask(TaskItemCreateDto taskDto)
        {
            var userId = GetUserId();
            var task = new TaskItem
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                IsCompleted = false,
                UserId = userId
            };

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();

            var responseDto = new TaskItemResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted
            };

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, responseDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskItemUpdateDto taskDto)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            var userId = GetUserId();

            if (task == null || task.UserId != userId)
            {
                return NotFound();
            }

            task.Title = taskDto.Title;
            task.Description = taskDto.Description;
            task.IsCompleted = taskDto.IsCompleted;

            _taskRepository.Update(task);
            await _taskRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            var userId = GetUserId();

            if (task == null || task.UserId != userId)
            {
                return NotFound();
            }

            _taskRepository.Remove(task);
            await _taskRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
