namespace Oslofjord.AdminDashboard.Contracts.Dtos;

public class CreateRoomTypeDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
    public string? ImageUrl { get; set; }
    public List<string>? Amenities { get; set; }
}

public class UpdateRoomTypeDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? Capacity { get; set; }
    public decimal? PricePerNight { get; set; }
    public string? ImageUrl { get; set; }
    public List<string>? Amenities { get; set; }
    public bool? IsActive { get; set; }
}

public class CreateAdditionDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Type { get; set; }
    public string? ImageUrl { get; set; }
    public int? MaxQuantity { get; set; }
}

public class UpdateAdditionDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? Type { get; set; }
    public string? ImageUrl { get; set; }
    public int? MaxQuantity { get; set; }
    public bool? IsActive { get; set; }
}

public class CreatePackageDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal TotalPrice { get; set; }
    public string? ImageUrl { get; set; }
    public List<string>? RoomTypeIds { get; set; }
    public List<PackageAdditionDto>? Additions { get; set; }
    public List<string>? EventIds { get; set; }
    public int MinParticipants { get; set; }
    public int MaxParticipants { get; set; }
    public int DurationDays { get; set; }
}

public class PackageAdditionDto
{
    public required string AdditionId { get; set; }
    public int Quantity { get; set; }
}
