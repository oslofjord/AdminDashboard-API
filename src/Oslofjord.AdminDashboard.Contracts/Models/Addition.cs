namespace Oslofjord.AdminDashboard.Contracts.Models;

public class Addition
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public AdditionType Type { get; set; }
    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }
    public int? MaxQuantity { get; set; }
    
    // Metadata
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public enum AdditionType
{
    Food,
    Beverage,
    Equipment,
    Service,
    Transport,
    Other
}
