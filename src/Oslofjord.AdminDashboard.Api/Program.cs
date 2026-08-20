using Oslofjord.AdminDashboard.Api.Configuration;
using Oslofjord.AdminDashboard.Api.Services;

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
        Description = "BFF (Backend for Frontend) for AdminDashboard - proxies requests to events-central-api"
    });
});

// Configure settings
var centralApiSettings = builder.Configuration.GetSection(CentralApiSettings.SectionName).Get<CentralApiSettings>();
builder.Services.Configure<CentralApiSettings>(builder.Configuration.GetSection(CentralApiSettings.SectionName));

// Configure HttpClient for Central API
if (centralApiSettings != null)
{
    builder.Services.AddHttpClient<ICentralApiService, CentralApiService>(client =>
    {
        client.BaseAddress = new Uri(centralApiSettings.BaseUrl);
        if (!string.IsNullOrEmpty(centralApiSettings.ApiKey))
        {
            client.DefaultRequestHeaders.Add("X-API-Key", centralApiSettings.ApiKey);
        }
        client.Timeout = TimeSpan.FromSeconds(centralApiSettings.TimeoutSeconds);
    });
}
else
{
    // Default configuration if settings not found
    builder.Services.AddHttpClient<ICentralApiService, CentralApiService>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5100");
        client.Timeout = TimeSpan.FromSeconds(30);
    });
}

// Register services
// No additional services needed - CentralApiService handles everything

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
