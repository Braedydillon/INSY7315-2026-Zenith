using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Text.Json;

namespace APIINSY7315.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly FirestoreDb _firestore;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            FirestoreDb firestore,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _firestore = firestore;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

     

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    error = "INVALID_REQUEST",
                    message = "Request body is required."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new
                {
                    error = "INVALID_EMAIL",
                    message = "Email is required."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Password) ||
                request.Password.Length < 6)
            {
                return BadRequest(new
                {
                    error = "INVALID_PASSWORD",
                    message = "Password must contain at least 6 characters."
                });
            }

            if (string.IsNullOrWhiteSpace(request.FullName))
            {
                return BadRequest(new
                {
                    error = "INVALID_NAME",
                    message = "Full name is required."
                });
            }

            if (string.IsNullOrWhiteSpace(request.IdNumber) || request.IdNumber.Length != 13)
            {
                return BadRequest(new
                {
                    error = "INVALID_ID_NUMBER",
                    message = "A valid 13-digit ID number is required."
                });
            }

            if (string.IsNullOrWhiteSpace(request.CellNo))
            {
                return BadRequest(new
                {
                    error = "INVALID_CELL_NO",
                    message = "Cell number is required."
                });
            }

            var email = request.Email.Trim().ToLowerInvariant();
            var fullName = request.FullName.Trim();

            try
            {
                FirebaseAuth auth = FirebaseAuth.DefaultInstance;

            

                try
                {
                    var existingUser =
                        await auth.GetUserByEmailAsync(email);

                    return Conflict(new
                    {
                        error = "EMAIL_ALREADY_EXISTS",
                        message = "An account with this email already exists.",
                        uid = existingUser.Uid
                    });
                }
                catch (FirebaseAuthException ex)
                {
                    if (ex.AuthErrorCode != AuthErrorCode.UserNotFound)
                    {
                        throw;
                    }
                }

              

                var firebaseUser =
                    await auth.CreateUserAsync(
                        new UserRecordArgs
                        {
                            Email = email,
                            Password = request.Password,
                            DisplayName = fullName,
                            EmailVerified = false
                        });

          

                const string role = "client";

              

                await auth.SetCustomUserClaimsAsync(
                    firebaseUser.Uid,
                    new Dictionary<string, object>
                    {
                        ["role"] = role
                    });

               

                var userData =
                    new Dictionary<string, object>
                    {
                        ["uid"] = firebaseUser.Uid,
                        ["email"] = email,
                        ["fullName"] = fullName,
                        ["idNumber"] = request.IdNumber.Trim(),
                        ["cellNo"] = request.CellNo.Trim(),
                        ["role"] = role,
                        ["createdAt"] =
                            Timestamp.GetCurrentTimestamp()
                    };

                await _firestore
                    .Collection("Users")
                    .Document(firebaseUser.Uid)
                    .SetAsync(
                        userData,
                        SetOptions.MergeAll);

              

                var idToken =
                    await FirebaseAuth.DefaultInstance
                        .CreateCustomTokenAsync(
                            firebaseUser.Uid,
                            new Dictionary<string, object>
                            {
                                ["role"] = role
                            });

                var tokenResult =
                    await ExchangeCustomTokenForIdTokenAsync(
                        idToken);

                return Ok(
                    new
                    {
                        idToken = tokenResult.IdToken,
                        refreshToken = tokenResult.RefreshToken,
                        expiresInSeconds = tokenResult.ExpiresIn,
                        uid = firebaseUser.Uid,
                        email = email,
                        role = role
                    });
            }
            catch (FirebaseAuthException ex)
            {
                _logger.LogError(
                    ex,
                    "Firebase registration failed for {Email}",
                    email);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        error = "FIREBASE_REGISTER_ERROR",
                        message = ex.Message
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Registration failed for {Email}",
                    email);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        error = "REGISTER_ERROR",
                        message = ex.Message
                    });
            }
        }

    

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    error = "INVALID_REQUEST",
                    message = "Email and password are required."
                });
            }

            try
            {
                var apiKey =
                    GetFirebaseApiKey();

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        new
                        {
                            error = "FIREBASE_API_KEY_MISSING",
                            message =
                                "Firebase Web API key is not configured."
                        });
                }

                var client =
                    _httpClientFactory.CreateClient();

                var url =
                    $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}";

                var payload =
                    new
                    {
                        email = request.Email.Trim(),
                        password = request.Password,
                        returnSecureToken = true
                    };

                var response =
                    await client.PostAsJsonAsync(
                        url,
                        payload);

                var json =
                    await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return Unauthorized(
                        new
                        {
                            error = "LOGIN_FAILED",
                            message = ExtractFirebaseError(json)
                        });
                }

                using var document =
                    JsonDocument.Parse(json);

                var root =
                    document.RootElement;

                var idToken =
                    root.GetProperty("idToken")
                        .GetString();

                var refreshToken =
                    root.GetProperty("refreshToken")
                        .GetString();

                var expiresIn =
                    root.GetProperty("expiresIn")
                        .GetString();

                var uid =
                    root.GetProperty("localId")
                        .GetString();

                var email =
                    root.GetProperty("email")
                        .GetString();

                var role = "client";

                if (!string.IsNullOrWhiteSpace(uid))
                {
                    var userSnapshot =
                        await _firestore
                            .Collection("Users")
                            .Document(uid)
                            .GetSnapshotAsync();

                    if (userSnapshot.Exists &&
                        userSnapshot.ContainsField("role"))
                    {
                        role =
                            userSnapshot
                                .GetValue<string>("role");
                    }
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
                        uid,
                        email,
                        role
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Login failed.");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        error = "LOGIN_ERROR",
                        message = ex.Message
                    });
            }
        }
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh(
            [FromBody] RefreshRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return BadRequest(new
                {
                    error = "INVALID_REFRESH_TOKEN",
                    message = "Refresh token is required."
                });
            }

            try
            {
                var apiKey =
                    GetFirebaseApiKey();

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        new
                        {
                            error = "FIREBASE_API_KEY_MISSING",
                            message =
                                "Firebase Web API key is not configured."
                        });
                }

                var client =
                    _httpClientFactory.CreateClient();

                var url =
                    $"https://securetoken.googleapis.com/v1/token?key={apiKey}";

                var content =
                    new FormUrlEncodedContent(
                        new Dictionary<string, string>
                        {
                            ["grant_type"] =
                                "refresh_token",

                            ["refresh_token"] =
                                request.RefreshToken
                        });

                var response =
                    await client.PostAsync(
                        url,
                        content);

                var json =
                    await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return Unauthorized(
                        new
                        {
                            error = "REFRESH_FAILED",
                            message =
                                ExtractFirebaseError(json)
                        });
                }

                using var document =
                    JsonDocument.Parse(json);

                var root =
                    document.RootElement;

                var idToken =
                    root.GetProperty("id_token")
                        .GetString();

                var refreshToken =
                    root.GetProperty("refresh_token")
                        .GetString();

                var expiresIn =
                    root.GetProperty("expires_in")
                        .GetString();

                var uid =
                    root.GetProperty("user_id")
                        .GetString();

                var role = "client";

                if (!string.IsNullOrWhiteSpace(uid))
                {
                    var snapshot =
                        await _firestore
                            .Collection("Users")
                            .Document(uid)
                            .GetSnapshotAsync();

                    if (snapshot.Exists &&
                        snapshot.ContainsField("role"))
                    {
                        role =
                            snapshot
                                .GetValue<string>("role");
                    }
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
                        uid,
                        role
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Refresh token operation failed.");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        error = "REFRESH_ERROR",
                        message = ex.Message
                    });
            }
        }

      

        private string? GetFirebaseApiKey()
        {
            return
                _configuration["Firebase:WebApiKey"]
                ??
                Environment.GetEnvironmentVariable(
                    "FIREBASE_API_KEY");
        }

        

        private async Task<TokenExchangeResult>
            ExchangeCustomTokenForIdTokenAsync(
                string customToken)
        {
            var apiKey =
                GetFirebaseApiKey();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    "Firebase API key is missing.");
            }

            var client =
                _httpClientFactory.CreateClient();

            var url =
                $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithCustomToken?key={apiKey}";

            var payload =
                new
                {
                    token = customToken,
                    returnSecureToken = true
                };

            var response =
                await client.PostAsJsonAsync(
                    url,
                    payload);

            var json =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    ExtractFirebaseError(json));
            }

            using var document =
                JsonDocument.Parse(json);

            var root =
                document.RootElement;

            return new TokenExchangeResult
            {
                IdToken =
                    root.GetProperty("idToken")
                        .GetString() ?? "",

                RefreshToken =
                    root.GetProperty("refreshToken")
                        .GetString() ?? "",

                ExpiresIn =
                    int.TryParse(
                        root.GetProperty("expiresIn")
                            .GetString(),
                        out var seconds)
                        ? seconds
                        : 3600
            };
        }

      

        private static string ExtractFirebaseError(
            string json)
        {
            try
            {
                using var document =
                    JsonDocument.Parse(json);

                if (document.RootElement
                    .TryGetProperty(
                        "error",
                        out var error))
                {
                    if (error.TryGetProperty(
                        "message",
                        out var message))
                    {
                        return message.GetString()
                            ?? "Firebase authentication failed.";
                    }
                }
            }
            catch
            {
                // Ignore JSON parsing failure.
            }

            return "Firebase authentication failed.";
        }

      

        private sealed class TokenExchangeResult
        {
            public string IdToken { get; set; } = "";
            public string RefreshToken { get; set; } = "";
            public int ExpiresIn { get; set; }
        }
    }


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

        [Required]
        [RegularExpression(@"^\d{13}$", ErrorMessage = "ID number must be 13 digits.")]
        public string IdNumber { get; set; } = "";

        [Required]
        [RegularExpression(@"^\+?\d{9,15}$", ErrorMessage = "Invalid cell number.")]
        public string CellNo { get; set; } = "";
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