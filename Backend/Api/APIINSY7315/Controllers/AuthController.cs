using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;

namespace APIINSY7315.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _config;
        private readonly FirestoreDb _db;

        public AuthController(
            IHttpClientFactory httpFactory,
            IConfiguration config,
            FirestoreDb db)
        {
            _httpFactory = httpFactory;
            _config = config;
            _db = db;
        }


        // ============================================================
        // REGISTER
        // ============================================================

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var apiKey = WebApiKey();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return ConfigError();
            }

            UserRecord user;

            try
            {
                user =
                    await FirebaseAuth
                        .DefaultInstance
                        .CreateUserAsync(
                            new UserRecordArgs
                            {
                                Email =
                                    request.Email.Trim(),

                                Password =
                                    request.Password,

                                DisplayName =
                                    request.FullName.Trim(),

                                Disabled = false
                            });
            }
            catch (FirebaseAuthException ex)
                when (ex.AuthErrorCode ==
                      AuthErrorCode.EmailAlreadyExists)
            {
                return Conflict(
                    new
                    {
                        error =
                            "ACCOUNT_EXISTS",

                        message =
                            "An account with that email already exists."
                    });
            }
            catch (FirebaseAuthException ex)
            {
                return BadRequest(
                    new
                    {
                        error =
                            "FIREBASE_CREATE_USER_FAILED",

                        message =
                            ex.Message
                    });
            }


            // ========================================================
            // CREATE CLIENT ROLE
            // ========================================================

            await FirebaseAuth
                .DefaultInstance
                .SetCustomUserClaimsAsync(
                    user.Uid,
                    new Dictionary<string, object>
                    {
                        ["role"] =
                            Roles.Client
                    });


            // ========================================================
            // CREATE USER PROFILE
            // ========================================================

            await _db
                .Collection("Users")
                .Document(user.Uid)
                .SetAsync(
                    new Dictionary<string, object>
                    {
                        ["Uid"] =
                            user.Uid,

                        ["Email"] =
                            user.Email ?? request.Email.Trim(),

                        ["FullName"] =
                            request.FullName.Trim(),

                        ["Role"] =
                            Roles.Client,

                        ["CreatedAt"] =
                            Timestamp.GetCurrentTimestamp()
                    });


            // ========================================================
            // SIGN IN
            // ========================================================

            return await SignInAndRespond(
                apiKey,
                request.Email.Trim(),
                request.Password);
        }


        // ============================================================
        // LOGIN
        // ============================================================

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var apiKey = WebApiKey();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return ConfigError();
            }

            return await SignInAndRespond(
                apiKey,
                request.Email.Trim(),
                request.Password);
        }


        // ============================================================
        // REFRESH TOKEN
        // ============================================================

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(
            [FromBody] RefreshRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var apiKey = WebApiKey();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return ConfigError();
            }

            var http =
                _httpFactory.CreateClient();

            var form =
                new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                        ["grant_type"] =
                            "refresh_token",

                        ["refresh_token"] =
                            request.RefreshToken
                    });


            var response =
                await http.PostAsync(
                    "https://securetoken.googleapis.com/v1/token" +
                    $"?key={apiKey}",
                    form);


            if (!response.IsSuccessStatusCode)
            {
                var body =
                    await response.Content
                        .ReadAsStringAsync();

                return Unauthorized(
                    new
                    {
                        error =
                            "REFRESH_FAILED",

                        message =
                            "The refresh token is invalid or expired.",

                        detail =
                            body
                    });
            }


            using var document =
                JsonDocument.Parse(
                    await response.Content
                        .ReadAsStringAsync());


            var root =
                document.RootElement;


            var idToken =
                root.GetProperty("id_token")
                    .GetString()!;

            var refreshToken =
                root.GetProperty("refresh_token")
                    .GetString()!;

            var expiresIn =
                root.GetProperty("expires_in")
                    .GetString();


            return await BuildTokenResponse(
                idToken,
                refreshToken,
                expiresIn);
        }


        // ============================================================
        // SIGN IN
        // ============================================================

        private async Task<IActionResult>
            SignInAndRespond(
                string apiKey,
                string email,
                string password)
        {
            var http =
                _httpFactory.CreateClient();


            var payload =
                new
                {
                    email,
                    password,
                    returnSecureToken = true
                };


            var content =
                new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json");


            var response =
                await http.PostAsync(
                    "https://identitytoolkit.googleapis.com/v1/" +
                    $"accounts:signInWithPassword?key={apiKey}",
                    content);


            var responseBody =
                await response.Content
                    .ReadAsStringAsync();


            if (!response.IsSuccessStatusCode)
            {
                return Unauthorized(
                    new
                    {
                        error =
                            "LOGIN_FAILED",

                        message =
                            "Invalid email or password.",

                        detail =
                            responseBody
                    });
            }


            using var document =
                JsonDocument.Parse(responseBody);


            var root =
                document.RootElement;


            var idToken =
                root.GetProperty("idToken")
                    .GetString()!;

            var refreshToken =
                root.GetProperty("refreshToken")
                    .GetString()!;

            var expiresIn =
                root.GetProperty("expiresIn")
                    .GetString();


            return await BuildTokenResponse(
                idToken,
                refreshToken,
                expiresIn);
        }


        // ============================================================
        // BUILD TOKEN RESPONSE
        // ============================================================

        private async Task<IActionResult>
            BuildTokenResponse(
                string idToken,
                string refreshToken,
                string? expiresIn)
        {
            FirebaseToken decoded;

            try
            {
                decoded =
                    await FirebaseAuth
                        .DefaultInstance
                        .VerifyIdTokenAsync(
                            idToken);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        error =
                            "TOKEN_VERIFICATION_FAILED",

                        message =
                            ex.Message
                    });
            }


            string role = "";

            if (decoded.Claims.TryGetValue(
                    "role",
                    out var roleClaim))
            {
                role =
                    Roles.Normalize(
                        roleClaim?.ToString());
            }


            string? email = null;

            if (decoded.Claims.TryGetValue(
                    "email",
                    out var emailClaim))
            {
                email =
                    emailClaim?.ToString();
            }


            return Ok(
                new
                {
                    idToken,

                    refreshToken,

                    expiresInSeconds =
                        int.TryParse(
                            expiresIn,
                            out var seconds)
                            ? seconds
                            : 3600,

                    uid =
                        decoded.Uid,

                    email,

                    role
                });
        }


        // ============================================================
        // FIREBASE WEB API KEY
        // ============================================================

        private string? WebApiKey()
        {
            var key =
                _config["Firebase:WebApiKey"];

            return string.IsNullOrWhiteSpace(key)
                ? null
                : key.Trim();
        }


        private ObjectResult ConfigError()
        {
            return StatusCode(
                500,
                new
                {
                    error =
                        "FIREBASE_CONFIGURATION_ERROR",

                    message =
                        "The Firebase Web API key is missing. Configure Firebase__WebApiKey."
                });
        }
    }


    // ================================================================
    // REQUEST MODELS
    // ================================================================

    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        public string Password { get; set; } = "";

        [Required]
        [StringLength(150)]
        public string FullName { get; set; } = "";
    }


    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";
    }


    public class RefreshRequest
    {
        [Required]
        public string RefreshToken { get; set; } = "";
    }
}