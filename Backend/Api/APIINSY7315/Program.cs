using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// SERVICES
// ============================================================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // --------------------------------------------------------
    // Swagger / Firebase Bearer authentication
    // --------------------------------------------------------

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description =
                "Enter your Firebase ID token."
        });

    options.AddSecurityRequirement(
        document =>
            new OpenApiSecurityRequirement
            {
                [
                    new OpenApiSecuritySchemeReference(
                        "Bearer",
                        document)
                ] = []
            });
});

builder.Services.AddHttpClient();


// ============================================================
// FIREBASE CONFIGURATION
// ============================================================

var projectId =
    builder.Configuration["Firebase:ProjectId"]
    ?? Environment.GetEnvironmentVariable(
        "FIREBASE_PROJECT_ID")
    ?? "insy7315-37442";


GoogleCredential? credential = null;


// ============================================================
// RENDER - JSON CREDENTIALS
// ============================================================

var credentialsJson =
    Environment.GetEnvironmentVariable(
        "FIREBASE_CREDENTIALS_JSON");


if (!string.IsNullOrWhiteSpace(
        credentialsJson))
{
    credential =
        GoogleCredential.FromJson(
            credentialsJson);
}


// ============================================================
// RENDER - BASE64 CREDENTIALS
// ============================================================

if (credential == null)
{
    var credentialsBase64 =
        Environment.GetEnvironmentVariable(
            "FIREBASE_CREDENTIALS_BASE64");


    if (!string.IsNullOrWhiteSpace(
            credentialsBase64))
    {
        var json =
            Encoding.UTF8.GetString(
                Convert.FromBase64String(
                    credentialsBase64));

        credential =
            GoogleCredential.FromJson(
                json);
    }
}


// ============================================================
// LOCAL DEVELOPMENT
// ============================================================

if (credential == null)
{
    var localCredentialsPath =
        Path.Combine(
            builder.Environment.ContentRootPath,
            "cred",
            "firebase-credentials.json");


    if (File.Exists(
            localCredentialsPath))
    {
        credential =
            GoogleCredential.FromFile(
                localCredentialsPath);
    }
}


// ============================================================
// GOOGLE APPLICATION CREDENTIALS
// ============================================================

if (credential == null)
{
    var environmentPath =
        Environment.GetEnvironmentVariable(
            "GOOGLE_APPLICATION_CREDENTIALS");


    if (!string.IsNullOrWhiteSpace(
            environmentPath) &&
        File.Exists(environmentPath))
    {
        credential =
            GoogleCredential.FromFile(
                environmentPath);
    }
}


// ============================================================
// MAKE SURE FIREBASE CREDENTIALS EXIST
// ============================================================

if (credential == null)
{
    throw new InvalidOperationException(
        "Firebase credentials are not configured. " +
        "Set FIREBASE_CREDENTIALS_JSON, " +
        "FIREBASE_CREDENTIALS_BASE64, " +
        "or GOOGLE_APPLICATION_CREDENTIALS.");
}


// ============================================================
// FIREBASE ADMIN SDK
// ============================================================

if (FirebaseApp.DefaultInstance == null)
{
    FirebaseApp.Create(
        new AppOptions
        {
            Credential = credential,
            ProjectId = projectId
        });
}


// ============================================================
// FIRESTORE
// ============================================================

var firestore =
    new FirestoreDbBuilder
    {
        ProjectId = projectId,
        Credential = credential
    }.Build();


builder.Services.AddSingleton(
    firestore);


// ============================================================
// CORS
// ============================================================

builder.Services.AddCors(
    options =>
    {
        options.AddPolicy(
            "Frontend",
            policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
    });


// ============================================================
// BUILD APPLICATION
// ============================================================

var app =
    builder.Build();


// ============================================================
// SWAGGER
// ============================================================

app.UseSwagger();

app.UseSwaggerUI(
    options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "APIINSY7315 v1");

        options.RoutePrefix =
            string.Empty;
    });


// ============================================================
// CORS
// ============================================================

app.UseCors("Frontend");


// ============================================================
// HEALTH
// ============================================================

app.MapGet(
    "/health",
    () =>
    {
        return Results.Ok(
            new
            {
                status = "ok",
                service = "APIINSY7315",
                projectId = projectId
            });
    });


// ============================================================
// FIRESTORE HEALTH
// ============================================================

app.MapGet(
    "/health/firestore",
    async (FirestoreDb db) =>
    {
        try
        {
            await db
                .Collection("LoanApplications")
                .Limit(1)
                .GetSnapshotAsync();


            return Results.Ok(
                new
                {
                    status = "ok",
                    firestore = "connected",
                    projectId = projectId
                });
        }
        catch (Exception ex)
        {
            return Results.Json(
                new
                {
                    status = "error",
                    firestore = "disconnected",
                    projectId = projectId,
                    message = ex.Message
                },
                statusCode: 503);
        }
    });


// ============================================================
// CONTROLLERS
// ============================================================

app.MapControllers();


// ============================================================
// START
// ============================================================

app.Run();