using Oslofjord.AdminDashboard.Contracts.Models;

namespace Oslofjord.AdminDashboard.Api.Services;

public interface IMomentusService
{
    Task<IEnumerable<MomentusEvent>> GetEventsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<MomentusEvent?> GetEventByIdAsync(string momentusId);
    Task<IEnumerable<MomentusEvent>> SearchEventsAsync(string query);
}

public class MomentusService : IMomentusService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MomentusService> _logger;
    
    public MomentusService(HttpClient httpClient, ILogger<MomentusService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<IEnumerable<MomentusEvent>> GetEventsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var queryParams = new List<string>();
            if (startDate.HasValue)
                queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue)
                queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");
            
            var query = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var response = await _httpClient.GetAsync($"/api/events{query}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<List<MomentusEvent>>() ?? new List<MomentusEvent>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching events from Momentus API");
            throw;
        }
    }
    
    public async Task<MomentusEvent?> GetEventByIdAsync(string momentusId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/events/{momentusId}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<MomentusEvent>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching event {MomentusId} from Momentus API", momentusId);
            throw;
        }
    }
    
    public async Task<IEnumerable<MomentusEvent>> SearchEventsAsync(string query)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/events/search?q={Uri.EscapeDataString(query)}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<List<MomentusEvent>>() ?? new List<MomentusEvent>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching events in Momentus API with query: {Query}", query);
            throw;
        }
    }
}

public class MomentusEvent
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Location { get; set; }
    public string? Category { get; set; }
    public Dictionary<string, object>? AdditionalData { get; set; }
}
