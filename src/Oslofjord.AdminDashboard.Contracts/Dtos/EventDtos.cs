namespace Oslofjord.AdminDashboard.Contracts.Dtos;

public class CreateEventDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public int Type { get; set; }
    public bool IsBookable { get; set; }
    public int? MaxParticipants { get; set; }
    public decimal? BasePrice { get; set; }
}

public class UpdateEventDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public int? Type { get; set; }
    public int? Status { get; set; }
    public bool? IsBookable { get; set; }
    public int? MaxParticipants { get; set; }
    public decimal? BasePrice { get; set; }
}

public class EnrichEventDto
{
    public string? EnrichedDescription { get; set; }
    public List<string>? ImageGallery { get; set; }
    public Dictionary<string, string>? CustomProperties { get; set; }
}

public class ImportFromMomentusDto
{
    public required string MomentusId { get; set; }
    public bool AutoEnrich { get; set; }
}
