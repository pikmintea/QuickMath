namespace QuickMath.Shared.Models
{
    public class UserProfile
    {
        public string Uid { get; set; } = "";
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? PhotoUrl { get; set; }
    }
}