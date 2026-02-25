namespace MphokeToDoList.Api.Models.DTOs
{
    // DTO for creating a new task
    public class TaskCreateDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool Completed { get; set; } = false;
        public int? AssignedToUserId { get; set; } // optional
    }

    // DTO for updating a task
    public class TaskUpdateDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool Completed { get; set; }
        public int? AssignedToUserId { get; set; } // optional
    }
}