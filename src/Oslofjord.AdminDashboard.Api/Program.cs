using Microsoft.Azure.Cosmos;
using Oslofjord.AdminDashboard.Api.Configuration;
using Oslofjord.AdminDashboard.Api.Data;
using Oslofjord.AdminDashboard.Api.Services;
using Oslofjord.AdminDashboard.Contracts.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "AdminDashboard API", 
        Version = "v1",
        Description = "API for AdminDashboard - manages enriched events, room types, additions, and packages"
    });
});

// Configure settings
var cosmosDbSettings = builder.Configuration.GetSection(CosmosDbSettings.SectionName).Get<CosmosDbSettings>();
var externalApiSettings = builder.Configuration.GetSection(ExternalApiSettings.SectionName).Get<ExternalApiSettings>();

builder.Services.Configure<CosmosDbSettings>(builder.Configuration.GetSection(CosmosDbSettings.SectionName));
builder.Services.Configure<ExternalApiSettings>(builder.Configuration.GetSection(ExternalApiSettings.SectionName));

// Configure Cosmos DB
if (cosmosDbSettings != null)
{
    builder.Services.AddSingleton(sp =>
    {
        var cosmosClient = new CosmosClient(cosmosDbSettings.EndpointUri, cosmosDbSettings.PrimaryKey);
        return cosmosClient;
    });
    
    // Register repositories
    builder.Services.AddSingleton<ICosmosDbRepository<EnrichedEvent>>(sp =>
    {
        var cosmosClient = sp.GetRequiredService<CosmosClient>();
        var database = cosmosClient.GetDatabase(cosmosDbSettings.DatabaseName);
        var container = database.GetContainer(cosmosDbSettings.EventsContainerName);
        return new CosmosDbRepository<EnrichedEvent>(container);
    });
}

// Configure HttpClients for external APIs
if (externalApiSettings != null)
{
    builder.Services.AddHttpClient<IMomentusService, MomentusService>(client =>
    {
        client.BaseAddress = new Uri(externalApiSettings.MomentusApiUrl);
        if (!string.IsNullOrEmpty(externalApiSettings.MomentusApiKey))
        {
            client.DefaultRequestHeaders.Add("X-API-Key", externalApiSettings.MomentusApiKey);
        }
        client.Timeout = TimeSpan.FromSeconds(30);
    });
    
    builder.Services.AddHttpClient<IRmsService, RmsService>(client =>
    {
        client.BaseAddress = new Uri(externalApiSettings.RmsApiUrl);
        if (!string.IsNullOrEmpty(externalApiSettings.RmsApiKey))
        {
            client.DefaultRequestHeaders.Add("X-API-Key", externalApiSettings.RmsApiKey);
        }
        client.Timeout = TimeSpan.FromSeconds(30);
    });
}

// Register services
builder.Services.AddScoped<IEventService, EventService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "http://localhost:3002",  // AdminDashboard frontend
                "http://localhost:3001",  // Kabuki frontend
                "http://localhost:3000"   // Development
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Add Application Insights (optional - only if connection string is configured)
var appInsightsConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
if (!string.IsNullOrEmpty(appInsightsConnectionString))
{
    builder.Services.AddApplicationInsightsTelemetry();
}

// Add caching
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AdminDashboard API V1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
