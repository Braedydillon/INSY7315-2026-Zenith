using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace APIINSY7315
{
    public static class Roles
    {
        public const string Client = "client";
        public const string Admin = "admin";
        public const string Management = "management";

        public const string Staff = "staff";

        public static readonly string[] All =
        {
            Client,
            Admin,
            Management,
            Staff
        };

        public static readonly string[] StaffType =
        {
            Admin,
            Management,
            Staff
        };

        public static bool IsValid(string? role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return false;

            return All.Contains(
                role.Trim().ToLowerInvariant());
        }

        public static string Normalize(string? role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return "";

            var normalized =
                role.Trim().ToLowerInvariant();

            return IsValid(normalized)
                ? normalized
                : "";
        }

        public static bool IsStaff(string? role)
        {
            var normalized = Normalize(role);

            return StaffType.Contains(normalized);
        }
    }


    [AttributeUsage(
        AttributeTargets.Class |
        AttributeTargets.Method,
        AllowMultiple = true)]
    public class FirebaseAuthorizeAttribute
        : Attribute,
          IAsyncAuthorizationFilter
    {
        private readonly string[] _allowedRoles;

        public FirebaseAuthorizeAttribute(
            params string[] allowedRoles)
        {
            _allowedRoles =
                allowedRoles ?? Array.Empty<string>();
        }


        public async Task OnAuthorizationAsync(
            AuthorizationFilterContext context)
        {
            var httpContext =
                context.HttpContext;

            var authorization =
                httpContext.Request.Headers
                    .Authorization
                    .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(
                    authorization))
            {
                context.Result =
                    new UnauthorizedObjectResult(
                        new
                        {
                            error = "UNAUTHORIZED",
                            message =
                                "Authorization header is required.",
                            traceId =
                                httpContext.TraceIdentifier
                        });

                return;
            }


            if (!authorization.StartsWith(
                    "Bearer ",
                    StringComparison.OrdinalIgnoreCase))
            {
                context.Result =
                    new UnauthorizedObjectResult(
                        new
                        {
                            error = "UNAUTHORIZED",
                            message =
                                "Authorization header must contain a Bearer token.",
                            traceId =
                                httpContext.TraceIdentifier
                        });

                return;
            }


            var token =
                authorization[
                    "Bearer ".Length..]
                .Trim();


            if (string.IsNullOrWhiteSpace(token))
            {
                context.Result =
                    new UnauthorizedObjectResult(
                        new
                        {
                            error = "UNAUTHORIZED",
                            message =
                                "Firebase ID token is missing.",
                            traceId =
                                httpContext.TraceIdentifier
                        });

                return;
            }


            FirebaseToken decoded;

            try
            {
                decoded =
                    await FirebaseAuth
                        .DefaultInstance
                        .VerifyIdTokenAsync(
                            token,
                            true);
            }
            catch (FirebaseAuthException ex)
            {
                context.Result =
                    new UnauthorizedObjectResult(
                        new
                        {
                            error =
                                "INVALID_FIREBASE_TOKEN",

                            message =
                                "The Firebase ID token is invalid or expired.",

                            detail =
                                ex.Message,

                            traceId =
                                httpContext.TraceIdentifier
                        });

                return;
            }
            catch (Exception ex)
            {
                context.Result =
                    new UnauthorizedObjectResult(
                        new
                        {
                            error =
                                "FIREBASE_AUTH_ERROR",

                            message =
                                ex.Message,

                            traceId =
                                httpContext.TraceIdentifier
                        });

                return;
            }


            var userId =
                decoded.Uid;


            string? email = null;

            if (decoded.Claims.TryGetValue(
                    "email",
                    out var emailClaim))
            {
                email =
                    emailClaim?.ToString();
            }


            string? role = null;

            if (decoded.Claims.TryGetValue(
                    "role",
                    out var roleClaim))
            {
                role =
                    roleClaim?.ToString();
            }

            role =
                Roles.Normalize(role);

            if (string.IsNullOrWhiteSpace(role))
            {
                context.Result =
                    new ObjectResult(
                        new
                        {
                            error =
                                "ROLE_MISSING",

                            message =
                                "The Firebase user does not have a valid application role.",

                            uid =
                                userId,

                            traceId =
                                httpContext.TraceIdentifier
                        })
                    {
                        StatusCode = 403
                    };

                return;
            }


            httpContext.Items["UserId"] =
                userId;

            httpContext.Items["Role"] =
                role;

            httpContext.Items["Email"] =
                email;


          
            if (_allowedRoles.Length == 0)
            {
                return;
            }


            var allowed =
                _allowedRoles.Any(
                    required =>
                        string.Equals(
                            Roles.Normalize(required),
                            role,
                            StringComparison.OrdinalIgnoreCase));


            if (!allowed)
            {
                context.Result =
                    new ObjectResult(
                        new
                        {
                            error =
                                "FORBIDDEN",

                            message =
                                "Your Firebase role is not allowed to access this endpoint.",

                            role =
                                role,

                            requiredRoles =
                                _allowedRoles,

                            traceId =
                                httpContext.TraceIdentifier
                        })
                    {
                        StatusCode = 403
                    };

                return;
            }
        }
    }


    public static class HttpContextUserExtensions
    {
        public static string GetUserId(
            this HttpContext context)
        {
            if (context.Items.TryGetValue(
                    "UserId",
                    out var value) &&
                value is string userId &&
                !string.IsNullOrWhiteSpace(userId))
            {
                return userId;
            }

            return "";
        }


        public static string GetRole(
            this HttpContext context)
        {
            if (context.Items.TryGetValue(
                    "Role",
                    out var value) &&
                value is string role)
            {
                return role;
            }

            return "";
        }


        public static string? GetEmail(
            this HttpContext context)
        {
            if (context.Items.TryGetValue(
                    "Email",
                    out var value) &&
                value is string email)
            {
                return email;
            }

            return null;
        }
    }
}