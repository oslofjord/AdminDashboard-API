using System.Net.Http.Json;
using Oslofjord.AdminDashboard.Contracts.Models;
using Oslofjord.AdminDashboard.Contracts.Dtos;

namespace Oslofjord.AdminDashboard.Api.Services;

/// <summary>
/// Service to communicate with events-central-api
/// </summary>
public interface ICentralApiService
{
    Task<IEnumerable<EnrichedEvent>> GetEventsAsync();
    Task<EnrichedEvent?> GetEventByIdAsync(string id);
    Task<EnrichedEvent> CreateEventAsync(CreateEventDto eventDto);
    Task<EnrichedEvent> UpdateEventAsync(string id, UpdateEventDto eventDto);
    Task DeleteEventAsync(string id);
    Task<EnrichedEvent> EnrichEventAsync(string id, EnrichEventDto enrichDto);
    Task<EnrichedEvent> ImportFromMomentusAsync(ImportFromMomentusDto importDto);
    
    Task<IEnumerable<RoomType>> GetRoomTypesAsync();
    Task<IEnumerable<Addition>> GetAdditionsAsync();
    Task<IEnumerable<Package>> GetPackagesAsync();
}

public class CentralApiService : ICentralApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CentralApiService> _logger;
    
    public CentralApiService(HttpClient httpClient, ILogger<CentralApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<IEnumerable<EnrichedEvent>> GetEventsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/events");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<EnrichedEvent>>() ?? new List<EnrichedEvent>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching events from Central API");
            throw;
        }
    }
    
    public async Task<EnrichedEvent?> GetEventByIdAsync(string id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/events/{id}");
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<EnrichedEvent>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching event {EventId} from Central API", id);
            throw;
        }
    }
    
    public async Task<EnrichedEvent> CreateEventAsync(CreateEventDto eventDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/events", eventDto);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<EnrichedEvent>() 
                   ?? throw new InvalidOperationException("Failed to deserialize created event");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event in Central API");
            throw;
        }
    }
    
    public async Task<EnrichedEvent> UpdateEventAsync(string id, UpdateEventDto eventDto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/events/{id}", eventDto);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<EnrichedEvent>() 
                   ?? throw new InvalidOperationException("Failed to deserialize updated event");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event {EventId} in Central API", id);
            throw;
        }
    }
    
    public async Task DeleteEventAsync(string id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/events/{id}");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event {EventId} in Central API", id);
            throw;
        }
    }
    
    public async Task<EnrichedEvent> EnrichEventAsync(string id, EnrichEventDto enrichDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/events/{id}/enrich", enrichDto);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<EnrichedEvent>() 
                   ?? throw new InvalidOperationException("Failed to deserialize enriched event");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enriching event {EventId} in Central API", id);
            throw;
        }
    }
    
    public async Task<EnrichedEvent> ImportFromMomentusAsync(ImportFromMomentusDto importDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/events/import", importDto);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<EnrichedEvent>() 
                   ?? throw new InvalidOperationException("Failed to deserialize imported event");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing event from Momentus via Central API");
            throw;
        }
    }
    
    public async Task<IEnumerable<RoomType>> GetRoomTypesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/roomtypes");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<RoomType>>() ?? new List<RoomType>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching room types from Central API");
            throw;
        }
    }
    
    public async Task<IEnumerable<Addition>> GetAdditionsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/additions");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Addition>>() ?? new List<Addition>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching additions from Central API");
            throw;
        }
    }
    
    public async Task<IEnumerable<Package>> GetPackagesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/packages");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Package>>() ?? new List<Package>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching packages from Central API");
            throw;
        }
    }
}
