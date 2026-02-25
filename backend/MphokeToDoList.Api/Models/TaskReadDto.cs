namespace MphokeToDoList.Api.Models.DTOs
{
    public class TaskReadDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Completed { get; set; }
        public int CreatedByUserId { get; set; }
        public UserDto CreatedByUser { get; set; } = new();
        public int? AssignedToUserId { get; set; }
        public UserDto? AssignedToUser { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}