using Microsoft.Maui.Authentication;
using QuickMath.Shared.Services;

namespace QuickMath.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        public async Task<string?> SignInAsync()
        {
            System.Diagnostics.Debug.WriteLine("SignInAsync called");
            try
            {
                var authResult = await WebAuthenticator.Default.AuthenticateAsync(
                    new WebAuthenticatorOptions
                    {
                        Url = new Uri("https://accounts.google.com/o/oauth2/v2/auth" +
                            "?client_id=395552232993-q40kavjdlunlcph5d6re7eb7he469n2v.apps.googleusercontent.com" +
                            "&redirect_uri=quickmath://" +
                            "&response_type=id_token" +
                            "&scope=openid%20email%20profile" +
                            "&nonce=" + Guid.NewGuid().ToString()),
                        CallbackUrl = new Uri("quickmath://"),
                        PrefersEphemeralWebBrowserSession = true
                    });

                return authResult.Properties["id_token"];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WebAuthenticator failed: {ex}");
                return null;
            }
        }
    }
}