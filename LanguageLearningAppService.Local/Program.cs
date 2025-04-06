using Amazon.DynamoDBv2;
using LanguageLearningAppService.Extensions;
using LanguageLearningAppService.Lambda;
using LanguageLearningAppService.Local;
using LanguageLearningAppService.Tests.DynamoDbFixture;

var builder = WebApplication.CreateBuilder(args);

// 1. Load configuration (includes appsettings.json, environment variables, etc.)
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>();

foreach (var kvp in builder.Configuration.AsEnumerable())
{
    if (!string.IsNullOrEmpty(kvp.Value))
    {
        // Set each secret as an environment variable
        Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
    }
}

// 2. Register your core services and repos (AddCommonServices is from your snippet)
builder.Services.AddCommonServices(builder.Configuration);
builder.Services.AddSingleton(typeof(Functions));

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "cors_policy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "https://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()  // Allows all headers, including Authorization
            .AllowCredentials();  // Allows cookies and authorization headers
    });
});

// 3. Build the app
var app = builder.Build();
var client = app.Services.GetRequiredService<IAmazonDynamoDB>();

app.UseCors("cors_policy");

var tableManager = new TableManager(client);
await tableManager.CreateTablesAsync();

// 4. Resolve your Functions instance from DI
var functionsInstance = app.Services.GetRequiredService<Functions>();

// 5. Use your reflection-based helper to map Lambda methods to local endpoints
LambdaMapping.RegisterLambdaAnnotatedMethods(app, functionsInstance);
// 6. Run the local server
app.Run();