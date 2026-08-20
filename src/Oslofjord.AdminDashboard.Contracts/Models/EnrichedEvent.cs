namespace Oslofjord.AdminDashboard.Contracts.Models;

public class EnrichedEvent
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public EventType Type { get; set; }
    public EventStatus Status { get; set; }
    
    // Momentus integration
    public string? MomentusId { get; set; }
    public DateTime? LastSyncedFromMomentus { get; set; }
    
    // Enrichment data
    public string? EnrichedDescription { get; set; }
    public List<string> ImageGallery { get; set; } = new();
    public Dictionary<string, string> CustomProperties { get; set; } = new();
    
    // Booking configuration
    public bool IsBookable { get; set; }
    public int? MaxParticipants { get; set; }
    public decimal? BasePrice { get; set; }
    
    // Related data
    public List<string> RoomTypeIds { get; set; } = new();
    public List<string> AdditionIds { get; set; } = new();
    public List<string> PackageIds { get; set; } = new();
    
    // Metadata
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}

public enum EventType
{
    Conference,
    Workshop,
    Meeting,
    Social,
    Training,
    Other
}

public enum EventStatus
{
    Draft,
    Published,
    Archived,
    Cancelled
}
