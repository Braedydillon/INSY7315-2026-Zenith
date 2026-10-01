using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure Firebase and Firestore Credentials safely for both local and cloud
GoogleCredential credential;
string? firebaseJson = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_JSON");

if (!string.IsNullOrEmpty(firebaseJson))
{
    // Cloud / Render environment variable approach
    credential = GoogleCredential.FromJson(firebaseJson);
}
else
{
    // Local development file path approach
    string pathToKey = Path.Combine(builder.Environment.ContentRootPath, "cred", "firebase-credentials.json");
    credential = GoogleCredential.FromFile(pathToKey);
}

// 2. Initialize Firebase App if not already initialized
if (FirebaseApp.DefaultInstance == null)
{
    FirebaseApp.Create(new AppOptions()
    {
        Credential = credential
    });
}

// 3. Register FirestoreDb using the explicit credential
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