using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;

namespace APIINSY7315.Controllers
{
    /// <summary>
    /// Register / login / refresh. Both the MVC site and the Android app call these and never need to
    /// talk to Firebase directly. Open to everyone (no token needed) because this is how you GET a token.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _config;
        private readonly FirestoreDb _db;

        public AuthController(IHttpClientFactory httpFactory, IConfiguration config, FirestoreDb db)
        {
            _httpFactory = httpFactory;
            _config = config;
            _db = db;
        }

        // POST api/Auth/register  -> creates the Firebase account, gives it the "client" role, returns tokens
        // POST api/Auth/register  -> creates the Firebase account, gives it the "client" role, returns tokens
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            string? apiKey = WebApiKey(); // Use the existing helper method here!
            if (apiKey == null) return ConfigError();

            UserRecord user;
            try
            {
                user = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
                {
                    Email = req.Email.Trim(),
                    Password = req.Password,
                    DisplayName = req.FullName.Trim()
                });
            }
            catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.EmailAlreadyExists)
            {
                return Conflict(new { error = "An account with that email already exists." });
            }
            catch (FirebaseAuthException ex)
            {
                return BadRequest(new { error = "Could not create account.", detail = ex.Message });
            }

            // Everyone who registers is a client. Only an admin can promote them later.
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(user.Uid,
                new Dictionary<string, object> { { "role", Roles.Client } });

            // Profile document in Firestore, keyed by the Firebase UID (the same ID used to own loan applications).
            await _db.Collection("Users").Document(user.Uid).SetAsync(new Dictionary<string, object>
    {
        { "Uid", user.Uid },
        { "Email", user.Email },
        { "FullName", req.FullName.Trim() },
        { "Role", Roles.Client },
        { "CreatedAt", Timestamp.GetCurrentTimestamp() }
    });

            // Sign in straight away so the caller gets a token that already contains role = client.
            return await SignInAndRespond(apiKey, req.Email.Trim(), req.Password);
        }

        // POST api/Auth/login  -> returns idToken (send as Bearer), refreshToken, role
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            string? apiKey = WebApiKey();
            if (apiKey == null) return ConfigError();
            return await SignInAndRespond(apiKey, req.Email.Trim(), req.Password);
        }

        // POST api/Auth/refresh  -> idTokens expire after 1 hour; swap the refreshToken for a new idToken
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest req)
        {
            string? apiKey = WebApiKey();
            if (apiKey == null) return ConfigError();

            var http = _httpFactory.CreateClient();
            var form = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", req.RefreshToken }
            });

            var resp = await http.PostAsync($"https://securetoken.googleapis.com/v1/token?key={apiKey}", form);
            if (!resp.IsSuccessStatusCode) return Unauthorized(new { error = "Session expired. Please log in again." });

            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            string idToken = doc.RootElement.GetProperty("id_token").GetString()!;
            string newRefresh = doc.RootElement.GetProperty("refresh_token").GetString()!;
            return await BuildTokenResponse(idToken, newRefresh, doc.RootElement.GetProperty("expires_in").GetString());
        }

        // ---- helpers ----------------------------------------------------------------
        private async Task<IActionResult> SignInAndRespond(string apiKey, string email, string password)
        {
            var http = _httpFactory.CreateClient();
            var payload = new { email = email, password = password, returnSecureToken = true };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var resp = await http.PostAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}", content);

            if (!resp.IsSuccessStatusCode)
            {
                string errorContent = await resp.Content.ReadAsStringAsync();
                return Unauthorized(new { error = "Invalid email or password.", details = errorContent });
            }

            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            return await BuildTokenResponse(
                doc.RootElement.GetProperty("idToken").GetString()!,
                doc.RootElement.GetProperty("refreshToken").GetString()!,
                doc.RootElement.GetProperty("expiresIn").GetString());
        }

        private static async Task<IActionResult> BuildTokenResponse(string idToken, string refreshToken, string? expiresIn)
        {
            // Read uid / role out of the token so the app knows what to show.
            var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
            string role = Roles.Normalize(decoded.Claims.TryGetValue("role", out var r) ? r?.ToString() : null);
            string? email = decoded.Claims.TryGetValue("email", out var e) ? e?.ToString() : null;

            return new OkObjectResult(new
            {
                idToken,
                refreshToken,
                expiresInSeconds = int.TryParse(expiresIn, out var s) ? s : 3600,
                uid = decoded.Uid,
                email,
                role
            });
        }

        private string? WebApiKey()
        {
            var key = _config["Firebase:WebApiKey"];
            return string.IsNullOrWhiteSpace(key) ? null : key;
        }

        private ObjectResult ConfigError() =>
            StatusCode(500, new { error = "Server is missing the Firebase Web API key (Firebase__WebApiKey)." });
    }

    public class RegisterRequest
    {
        [Required, EmailAddress] public string Email { get; set; } = "";
        [Required, MinLength(6), MaxLength(100)] public string Password { get; set; } = "";
        [Required, StringLength(150)] public string FullName { get; set; } = "";
    }

    public class LoginRequest
    {
        [Required, EmailAddress] public string Email { get; set; } = "";
        [Required] public string Password { get; set; } = "";
    }

    public class RefreshRequest
    {
        [Required] public string RefreshToken { get; set; } = "";
    }
}
