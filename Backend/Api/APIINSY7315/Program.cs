using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.OpenApi;
using System.IO;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

GoogleCredential credential;
string? firebaseJson = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_JSON");
if (!string.IsNullOrEmpty(firebaseJson))
{
    credential = GoogleCredential.FromJson(firebaseJson);
}
else
{
    string pathToKey = Path.Combine(builder.Environment.ContentRootPath, "cred", "firebase-credentials.json");
    credential = GoogleCredential.FromFile(pathToKey);
}

if (FirebaseApp.DefaultInstance == null)
{
    FirebaseApp.Create(new AppOptions()
    {
        Credential = credential
    });
}

builder.Services.AddSingleton(provider =>
    FirestoreDb.Create("insy7315-37442", new Google.Cloud.Firestore.V1.FirestoreClientBuilder
    {
        Credential = credential
    }.Build()));

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIINSY7315", Version = "v1" });

    // Define the Bearer token scheme for OpenAPI v3
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document),
            new List<string>()
        }
    });
});
//rate limiter


builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;

    options.OnRejected = async (context, token) =>
    {
        var response = context.HttpContext.Response;
        response.StatusCode = 429;
        response.ContentType = "application/json";

        await response.WriteAsJsonAsync(new
        {
            error = "RATE_LIMIT_EXCEEDED",
            message = "Too many requests. Please try again later."
        }, token);
    };

    // Protect login against repeated attempts
    options.AddPolicy("login",
        ctx => LimitByIp(
            ctx, 5, TimeSpan.FromMinutes(1)));

    // Protect registration against spam
    options.AddPolicy("register",
        ctx => LimitByIp(
            ctx, 3, TimeSpan.FromHours(1)));

    // Protect token refresh
    options.AddPolicy("refresh",
        ctx => LimitByIp(
            ctx, 10, TimeSpan.FromMinutes(1)));

    // Loan application quotas are managed
    // separately per Firebase UID in Firestore.
});





var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FirestoreDb>();
    await SeedDefaultAdminAsync(db);
}

async Task SeedDefaultAdminAsync(FirestoreDb db)
{
    const string defaultAdminEmail = "admin@bridgeandanchor.com";
    const string defaultAdminPassword = "AdminPassword123!";
    const string defaultAdminName = "System Administrator";

    try
    {
        UserRecord? user = null;
        try
        {
            user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(defaultAdminEmail);
        }
        catch (FirebaseAuthException)
        {
        }

        if (user == null)
        {
            user = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
            {
                Email = defaultAdminEmail,
                Password = defaultAdminPassword,
                DisplayName = defaultAdminName,
                EmailVerified = true
            });
        }

        await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(
            user.Uid,
            new Dictionary<string, object> { { "role", "admin" } });

        var adminData = new Dictionary<string, object>
        {
            ["uid"] = user.Uid,
            ["email"] = defaultAdminEmail,
            ["fullName"] = defaultAdminName,
            ["role"] = "admin",
            ["createdAt"] = Timestamp.GetCurrentTimestamp()
        };

        await db.Collection("Users").Document(user.Uid).SetAsync(adminData, SetOptions.MergeAll);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error seeding default admin: {ex.Message}");
    }
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "APIINSY7315 v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthorization();
app.MapControllers();
app.Run();

//helper function for rate limiting

static RateLimitPartition<string> LimitByIp(
    HttpContext context,
    int limit,
    TimeSpan window)
{
    var ip = context.Connection.RemoteIpAddress?.ToString()
             ?? "unknown";

    return RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: ip,
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = limit,
            Window = window,
            QueueLimit = 0,
            AutoReplenishment = true
        });
}