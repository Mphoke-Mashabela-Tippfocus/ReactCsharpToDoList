using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MphokeToDoList.Api.Data;
using MphokeToDoList.Api.Models;
using MphokeToDoList.Api.Models.DTOs;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    // ================================
    // Helpers
    // ================================
    private bool TryGetCurrentUserId(out int userId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out userId);
    }

    private string GetCurrentUserRole()
    {
        return User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
    }

    // ================================
    // Get Tasks
    // ================================
    [HttpGet]
    public async Task<IActionResult> GetTasks()
    {
        if (!TryGetCurrentUserId(out var userId))
            return Unauthorized();

        var role = GetCurrentUserRole();

        IQueryable<TaskItem> query = _context.Tasks
                                             .Include(t => t.AssignedToUser)
                                             .Include(t => t.CreatedByUser);

        if (role != "Admin")
            query = query.Where(t => t.CreatedByUserId == userId);

        var tasks = await query
            .Select(t => new TaskReadDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Completed = t.Completed,
                CreatedByUserId = t.CreatedByUserId,
                CreatedByUser = new UserDto
                {
                    Id = t.CreatedByUser.Id,
                    Username = t.CreatedByUser.Username,
                    Role = t.CreatedByUser.Role
                },
                AssignedToUserId = t.AssignedToUserId,
                AssignedToUser = t.AssignedToUser == null ? null : new UserDto
                {
                    Id = t.AssignedToUser.Id,
                    Username = t.AssignedToUser.Username,
                    Role = t.AssignedToUser.Role
                }
            })
            .ToListAsync();

        return Ok(tasks);
    }

    // ================================
    // Create Task
    // ================================
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] TaskCreateDto dto)
    {
        if (!TryGetCurrentUserId(out var userId))
            return Unauthorized();

        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Completed = dto.Completed,
            CreatedByUserId = userId,
            AssignedToUserId = dto.AssignedToUserId
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, task);
    }

    // ================================
    // Update Task
    // ================================
    [HttpPut("{id}")]
    public async Task<IActionResult> EditTask(int id, [FromBody] TaskUpdateDto dto)
    {
        if (!TryGetCurrentUserId(out var userId))
            return Unauthorized();

        var role = GetCurrentUserRole();
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        if (role != "Admin" && task.CreatedByUserId != userId)
            return Forbid();

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Completed = dto.Completed;
        task.AssignedToUserId = dto.AssignedToUserId;

        await _context.SaveChangesAsync();
        return Ok(task);
    }

    // ================================
    // Delete Task (Admin Only)
    // ================================
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Task deleted successfully" });
    }

    // ================================
    // Assign Task (Admin Only)
    // ================================
    [HttpPost("{id}/assign/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignTask(int id, int userId)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return NotFound("Task not found");

        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists) return NotFound("User not found");

        task.AssignedToUserId = userId;
        await _context.SaveChangesAsync();

        return Ok(task);
    }
}