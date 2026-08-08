using QuickMath.Shared.Services;
using System.Net.Http.Json;

namespace QuickMath.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        // Same Web Client ID used by Android/Firebase — do not change.
        private const string WebClientId = Secrets.GoogleWebClientId;

        // Desktop app Client ID — used only by the Windows PKCE flow.
        private const string DesktopClientId = Secrets.GoogleDesktopClientId;

        // Google's token endpoint requires this even for "Desktop app" clients
        // using PKCE (a quirk of Google's implementation, not how PKCE is
        // supposed to work). Lives in the gitignored Secrets.cs.
        private const string DesktopClientSecret = Secrets.GoogleDesktopClientSecret;

        // Firebase Web API key — from google-services.json ("current_key").
        private const string FirebaseWebApiKey = Secrets.FirebaseWebApiKey;

        public async Task<string?> SignInAsync()
        {
            System.Diagnostics.Debug.WriteLine("SignInAsync called");
            try
            {
#if ANDROID
                return await SignInAndroidAsync();
#elif WINDOWS
                return await SignInWindowsAsync();
#else
                System.Diagnostics.Debug.WriteLine("Google Sign-In not implemented on this platform yet.");
                return null;
#endif
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SignInAsync failed: {ex}");
                return null;
            }
        }

        /// <summary>
        /// Signs in with Google, then exchanges the Google ID token for a Firebase
        /// user via Firebase Auth's REST API. Returns the signed-in user's profile
        /// (uid, name, email, photo) on success, or null on failure/cancellation.
        /// </summary>
        public async Task<QuickMath.Shared.Models.UserProfile?> SignInToFirebaseAsync()
        {
            var googleIdToken = await SignInAsync();
            if (googleIdToken == null)
            {
                return null;
            }

            try
            {
                using var http = new HttpClient();
                var payload = new
                {
                    postBody = $"id_token={googleIdToken}&providerId=google.com",
                    requestUri = "http://localhost", // required by the API, not actually used
                    returnSecureToken = true
                };

                var response = await http.PostAsJsonAsync(
                    $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithIdp?key={FirebaseWebApiKey}",
                    payload);

                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"Firebase sign-in failed: {json}");
                    return null;
                }

                using var doc = System.Text.Json.JsonDocument.Parse(json);
                var root = doc.RootElement;

                return new QuickMath.Shared.Models.UserProfile
                {
                    Uid = root.GetProperty("localId").GetString() ?? "",
                    DisplayName = root.TryGetProperty("displayName", out var dn) ? dn.GetString() : null,
                    Email = root.TryGetProperty("email", out var em) ? em.GetString() : null,
                    PhotoUrl = root.TryGetProperty("photoUrl", out var pu) ? pu.GetString() : null
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SignInToFirebaseAsync failed: {ex}");
                return null;
            }
        }

#if ANDROID
        private static async Task<string?> SignInAndroidAsync()
        {
            var authResult = await Microsoft.Maui.Authentication.WebAuthenticator.Default.AuthenticateAsync(
                new Microsoft.Maui.Authentication.WebAuthenticatorOptions
                {
                    Url = new Uri("https://accounts.google.com/o/oauth2/v2/auth" +
                        $"?client_id={WebClientId}" +
                        "&redirect_uri=quickmath://" +
                        "&response_type=id_token" +
                        "&scope=openid%20email%20profile" +
                        "&nonce=" + Guid.NewGuid()),
                    CallbackUrl = new Uri("quickmath://"),
                    PrefersEphemeralWebBrowserSession = true
                });

            return authResult.Properties["id_token"];
        }
#endif

#if WINDOWS
        // Windows has no native WebAuthenticator support, so we do the OAuth
        // "loopback + PKCE" flow ourselves:
        // 1. Generate a PKCE code_verifier/code_challenge pair (no client secret needed).
        // 2. Start a tiny local HTTP listener on 127.0.0.1 on a free port.
        // 3. Open the system browser to Google's consent screen, redirect_uri pointing at that listener.
        // 4. Wait for Google to redirect back with an authorization "code".
        // 5. Exchange that code (+ code_verifier) for tokens via Google's token endpoint.
        private static async Task<string?> SignInWindowsAsync()
        {
            var codeVerifier = GeneratePkceCodeVerifier();
            var codeChallenge = GeneratePkceCodeChallenge(codeVerifier);

            using var listener = new System.Net.HttpListener();
            var port = GetFreeTcpPort();
            var redirectUri = $"http://127.0.0.1:{port}/callback/";
            listener.Prefixes.Add(redirectUri);
            listener.Start();

            var authUrl = "https://accounts.google.com/o/oauth2/v2/auth" +
                $"?client_id={DesktopClientId}" +
                $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                "&response_type=code" +
                "&scope=openid%20email%20profile" +
                $"&code_challenge={codeChallenge}" +
                "&code_challenge_method=S256";

            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = authUrl,
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);

            // Wait for Google to redirect back to our local listener.
            var context = await listener.GetContextAsync();
            var query = System.Web.HttpUtility.ParseQueryString(context.Request.Url?.Query ?? "");
            var code = query["code"];

            // Respond to the browser so the tab doesn't hang.
            var responseString = "<html><body>Signed in — you can close this tab and return to QuickMath.</body></html>";
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer);
            context.Response.OutputStream.Close();
            listener.Stop();

            if (string.IsNullOrEmpty(code))
            {
                System.Diagnostics.Debug.WriteLine("No authorization code returned from Google.");
                return null;
            }

            return await ExchangeCodeForIdTokenAsync(code, codeVerifier, redirectUri);
        }

        private static async Task<string?> ExchangeCodeForIdTokenAsync(string code, string codeVerifier, string redirectUri)
        {
            using var http = new HttpClient();
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = DesktopClientId,
                ["client_secret"] = DesktopClientSecret,
                ["redirect_uri"] = redirectUri,
                ["grant_type"] = "authorization_code",
                ["code_verifier"] = codeVerifier
            });

            var response = await http.PostAsync("https://oauth2.googleapis.com/token", content);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine($"Token exchange failed: {json}");
                return null;
            }

            using var doc = System.Text.Json.JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("id_token").GetString();
        }

        private static string GeneratePkceCodeVerifier()
        {
            var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes)
                .TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        private static string GeneratePkceCodeChallenge(string codeVerifier)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var challengeBytes = sha256.ComputeHash(System.Text.Encoding.ASCII.GetBytes(codeVerifier));
            return Convert.ToBase64String(challengeBytes)
                .TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        private static int GetFreeTcpPort()
        {
            var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
            listener.Start();
            var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
#endif
    }
}