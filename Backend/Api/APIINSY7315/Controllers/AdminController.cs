using System.ComponentModel.DataAnnotations;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;

namespace APIINSY7315.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [FirebaseAuthorize(Roles.Admin)] // admins only
    public class AdminController : ControllerBase
    {
        private readonly FirestoreDb _db;
        public AdminController(FirestoreDb db) => _db = db;

        // POST api/Admin/set-role
        [HttpPost("set-role")]
        public async Task<IActionResult> SetUserRole([FromBody] RoleAssignmentRequest request)
        {
            var role = request.Role.Trim().ToLowerInvariant();
            if (!Roles.All.Contains(role))
                return BadRequest(new { error = "Role must be one of: " + string.Join(", ", Roles.All) });

            UserRecord user;
            try { user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(request.Email); }
            catch (FirebaseAuthException) { return NotFound(new { error = "No user with that email." }); }

            if (user.Uid == HttpContext.GetUserId())
                return BadRequest(new { error = "You cannot change your own role." });

            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(user.Uid,
                new Dictionary<string, object> { { "role", role } });

            // Keep the readable profile in Firestore in sync (the token claim remains the source of truth).
            await _db.Collection("Users").Document(user.Uid)
                .SetAsync(new Dictionary<string, object> { { "Role", role } }, SetOptions.MergeAll);

            // Forces the user's apps to re-authenticate so the new role takes effect straight away.
            await FirebaseAuth.DefaultInstance.RevokeRefreshTokensAsync(user.Uid);

            return Ok(new { message = $"'{request.Email}' is now '{role}'." });
        }

        // GET api/Admin/users - list accounts and their roles (for an admin screen)
        [HttpGet("users")]
        public async Task<IActionResult> ListUsers()
        {
            var result = new List<object>();
            var pages = FirebaseAuth.DefaultInstance.ListUsersAsync(null).AsRawResponses().GetAsyncEnumerator();
            while (await pages.MoveNextAsync())
            {
                foreach (var u in pages.Current.Users)
                {
                    u.CustomClaims.TryGetValue("role", out var r);
                    result.Add(new { uid = u.Uid, email = u.Email, role = Roles.Normalize(r?.ToString()), disabled = u.Disabled });
                }
            }
            return Ok(result);
        }
    }

    public class RoleAssignmentRequest
    {
        [Required, EmailAddress] public string Email { get; set; } = "";
        [Required] public string Role { get; set; } = ""; // admin | management | client
    }
}
