namespace Oslofjord.AdminDashboard.Api.Configuration;

public class ExternalApiSettings
{
    public const string SectionName = "ExternalApis";
    
    public required string MomentusApiUrl { get; set; }
    public required string RmsApiUrl { get; set; }
    public required string PersonsApiUrl { get; set; }
    public required string EventsApiUrl { get; set; }
    
    public string? MomentusApiKey { get; set; }
    public string? RmsApiKey { get; set; }
}
