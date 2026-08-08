using QuickMath.Shared.Models;

namespace QuickMath.Shared.Services
{
    public interface IGoogleAuthService
    {
        Task<string?> SignInAsync();
        Task<UserProfile?> SignInToFirebaseAsync();
    }
}