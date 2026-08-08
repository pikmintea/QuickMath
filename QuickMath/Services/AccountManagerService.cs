using QuickMath.Shared.Services;
using QuickMath.Shared.Models;
using System;
using System.Text.Json;

namespace QuickMath.Services
{
    public class AccountManagerService : IAccountManagerService
    {
        private const string AccountStateKey = "account_state";
        private const string UserProfileKey = "user_profile";

        public AccountState State { get; private set; }
        public UserProfile? CurrentUser { get; private set; }

        public AccountManagerService()
        {
            var savedState = Preferences.Get(AccountStateKey, nameof(AccountState.NotSignedInOrGuest));
            State = Enum.Parse<AccountState>(savedState);

            var savedProfileJson = Preferences.Get(UserProfileKey, string.Empty);
            if (!string.IsNullOrEmpty(savedProfileJson))
            {
                try
                {
                    CurrentUser = JsonSerializer.Deserialize<UserProfile>(savedProfileJson);
                }
                catch
                {
                    CurrentUser = null;
                }
            }
        }

        public void SetGuest()
        {
            State = AccountState.Guest;
            CurrentUser = null;
            Preferences.Set(AccountStateKey, nameof(AccountState.Guest));
            Preferences.Remove(UserProfileKey);
        }

        public void SetSignedIn(UserProfile profile)
        {
            State = AccountState.SignedIn;
            CurrentUser = profile;
            Preferences.Set(AccountStateKey, nameof(AccountState.SignedIn));
            Preferences.Set(UserProfileKey, JsonSerializer.Serialize(profile));
        }
        public void ClearAllData()
        {
            Preferences.Clear();
            State = AccountState.NotSignedInOrGuest;
            CurrentUser = null;
        }
    }
}