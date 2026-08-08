using System;
using System.Collections.Generic;
using System.Text;

namespace QuickMath.Shared.Services
{
    public interface IAccountManagerService
    {
        AccountState State { get; }
        void SetGuest();
        void SetSignedIn();
    }
}