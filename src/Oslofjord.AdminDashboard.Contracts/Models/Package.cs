namespace Oslofjord.AdminDashboard.Contracts.Models;

public class Package
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }
    
    // Package contents
    public List<string> RoomTypeIds { get; set; } = new();
    public List<PackageAddition> Additions { get; set; } = new();
    public List<string> EventIds { get; set; } = new();
    
    // Configuration
    public int MinParticipants { get; set; }
    public int MaxParticipants { get; set; }
    public int DurationDays { get; set; }
    
    // Metadata
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class PackageAddition
{
    public required string AdditionId { get; set; }
    public int Quantity { get; set; }
}
