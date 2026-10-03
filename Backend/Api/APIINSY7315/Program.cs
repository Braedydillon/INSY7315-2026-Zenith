using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using System.IO;

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
builder.Services.AddSwaggerGen();










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
app.UseAuthorization();
app.MapControllers();
app.Run();