namespace MphokeToDoList.Api.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool Completed { get; set; } = false;

        // Foreign key to the user who created the task
        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = null!;

        // Foreign key to the assigned user (optional)
        public int? AssignedToUserId { get; set; }
        public User? AssignedToUser { get; set; }
    }
}