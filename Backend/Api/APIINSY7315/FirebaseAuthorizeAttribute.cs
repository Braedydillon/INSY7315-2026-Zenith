using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public static class Roles
{
    public const string Admin = "admin";
    public const string Management = "management";
    public const string Client = "client";

    public static readonly string[] All = { Admin, Management, Client };
    public static readonly string[] Staff = { Admin, Management };

    // Unknown or missing role always degrades to the least-privileged role.
    public static string Normalize(string? role)
    {
        var r = role?.Trim().ToLowerInvariant();
        return r != null && All.Contains(r) ? r : Client;
    }

    public static bool IsStaff(string role) => Staff.Contains(role);
}

/// <summary>
/// Verifies the Firebase ID token (same token from web and mobile) and checks the role claim.
/// [FirebaseAuthorize] = any signed-in user; [FirebaseAuthorize(Roles.Admin)] = admins only.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class FirebaseAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string[] _allowedRoles;

    public FirebaseAuthorizeAttribute(params string[] allowedRoles) => _allowedRoles = allowedRoles;

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var authHeader = context.HttpContext.Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        string token = authHeader["Bearer ".Length..].Trim();

        FirebaseToken decoded;
        try
        {
            // checkRevoked = true so disabled users / revoked sessions are rejected immediately.
            decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token, true);
        }
        catch
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        string role = Roles.Normalize(
            decoded.Claims.TryGetValue("role", out var r) ? r?.ToString() : null);

        context.HttpContext.Items["UserId"] = decoded.Uid;
        context.HttpContext.Items["Role"] = role;
        context.HttpContext.Items["Email"] = decoded.Claims.TryGetValue("email", out var e) ? e?.ToString() : null;

        // Method-level and class-level attributes both run; each must pass.
        if (_allowedRoles.Length > 0 && !_allowedRoles.Contains(role))
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
    }
}

public static class HttpContextUserExtensions
{
    public static string GetUserId(this HttpContext c) => (string)c.Items["UserId"]!;
    public static string GetRole(this HttpContext c) => (string)c.Items["Role"]!;
    public static string? GetEmail(this HttpContext c) => c.Items["Email"] as string;
}