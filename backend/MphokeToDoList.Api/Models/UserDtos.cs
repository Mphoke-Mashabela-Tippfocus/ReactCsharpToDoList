namespace MphokeToDoList.Api.Models.DTOs
{
    public class UserLoginDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class UserRegisterDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; } // optional
        public DateTime? DateOfBirth { get; set; } // optional
        // Role is not included; defaulted to "User"
    }
}