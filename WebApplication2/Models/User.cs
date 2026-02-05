namespace WebApplication2.Models
{
    public class User
    {
        public enum UserStatus
        {
        Unverified = 0,
        Active =1,
        Blocked =2
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? ConfirmToken { get; set; }

        public string Password { get; set; } = null!;
        public UserStatus Status { get; set; } = UserStatus.Unverified;

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public DateTime? LastActivityAt{ get; set; }
    }
}
