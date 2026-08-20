namespace Oslofjord.AdminDashboard.Contracts.Models;

public class RoomType
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
    public string? ImageUrl { get; set; }
    public List<string> Amenities { get; set; } = new();
    
    // RMS integration
    public string? RmsId { get; set; }
    public DateTime? LastSyncedFromRms { get; set; }
    
    // Availability
    public bool IsActive { get; set; }
    public int AvailableRooms { get; set; }
    
    // Metadata
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
