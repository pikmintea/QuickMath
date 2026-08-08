using System;
using System.Collections.Generic;
using System.Text;

namespace QuickMath.Shared.Services
{
    public interface IGoogleAuthService
    {
        Task<string?> SignInAsync(); 
    }
}