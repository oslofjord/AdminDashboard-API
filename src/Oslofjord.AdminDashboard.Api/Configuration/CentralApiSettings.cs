namespace Oslofjord.AdminDashboard.Api.Configuration;

public class CentralApiSettings
{
    public const string SectionName = "CentralApi";
    
    public required string BaseUrl { get; set; }
    public string? ApiKey { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
}
