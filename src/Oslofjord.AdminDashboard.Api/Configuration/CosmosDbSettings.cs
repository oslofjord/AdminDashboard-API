namespace Oslofjord.AdminDashboard.Api.Configuration;

public class CosmosDbSettings
{
    public const string SectionName = "CosmosDb";
    
    public required string EndpointUri { get; set; }
    public required string PrimaryKey { get; set; }
    public required string DatabaseName { get; set; }
    
    public string EventsContainerName { get; set; } = "EnrichedEvents";
    public string RoomTypesContainerName { get; set; } = "RoomTypes";
    public string AdditionsContainerName { get; set; } = "Additions";
    public string PackagesContainerName { get; set; } = "Packages";
}
