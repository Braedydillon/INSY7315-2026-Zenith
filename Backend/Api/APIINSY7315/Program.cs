using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// 1. Initialize Firebase Admin SDK
string pathToKey = Path.Combine(builder.Environment.ContentRootPath, "cred", "firebase-credentials.json");
if (File.Exists(pathToKey) && FirebaseApp.DefaultInstance == null)
{
    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromFile(pathToKey)
    });
}

// 2. Set environment variable and register FirestoreDb
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", pathToKey);
builder.Services.AddSingleton(FirestoreDb.Create("insy7315-37442")); // Ensure this matches your Firebase Project ID from your config

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