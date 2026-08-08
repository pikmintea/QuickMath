using QuickMath.Shared.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickMath.Services
{


    public class AccountManagerService : IAccountManagerService
    {
        private const string AccountStateKey = "account_state";
        public AccountState State { get; private set; }

        public AccountManagerService()
        {
            var saved = Preferences.Get(AccountStateKey, nameof(AccountState.NotSignedInOrGuest));
            State = Enum.Parse<AccountState>(saved);
        }

        public void SetGuest()
        {
            State = AccountState.Guest;
            Preferences.Set(AccountStateKey, nameof(AccountState.Guest));
        }

        public void SetSignedIn()
        {
            State = AccountState.SignedIn;
            Preferences.Set(AccountStateKey, nameof(AccountState.SignedIn));
        }

    }
}
