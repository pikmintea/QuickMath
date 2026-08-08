using QuickMath.Shared.Models;

namespace QuickMath.Shared.Services
{
    public interface IAccountManagerService
    {
        AccountState State { get; }
        UserProfile? CurrentUser { get; }
        void SetGuest();
        void SetSignedIn(UserProfile profile);

        void ClearAllData();
    }
}