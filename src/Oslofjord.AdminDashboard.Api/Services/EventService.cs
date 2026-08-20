using Oslofjord.AdminDashboard.Contracts.Models;
using Oslofjord.AdminDashboard.Api.Data;

namespace Oslofjord.AdminDashboard.Api.Services;

public interface IEventService
{
    Task<IEnumerable<EnrichedEvent>> GetAllEventsAsync();
    Task<EnrichedEvent?> GetEventByIdAsync(string id);
    Task<EnrichedEvent> CreateEventAsync(EnrichedEvent eventData);
    Task<EnrichedEvent> UpdateEventAsync(string id, EnrichedEvent eventData);
    Task DeleteEventAsync(string id);
    Task<EnrichedEvent> EnrichEventAsync(string id, string? description, List<string>? images, Dictionary<string, string>? customProperties);
    Task<EnrichedEvent> ImportFromMomentusAsync(string momentusId, bool autoEnrich);
}

public class EventService : IEventService
{
    private readonly ICosmosDbRepository<EnrichedEvent> _repository;
    private readonly IMomentusService _momentusService;
    private readonly ILogger<EventService> _logger;
    
    public EventService(
        ICosmosDbRepository<EnrichedEvent> repository,
        IMomentusService momentusService,
        ILogger<EventService> logger)
    {
        _repository = repository;
        _momentusService = momentusService;
        _logger = logger;
    }
    
    public async Task<IEnumerable<EnrichedEvent>> GetAllEventsAsync()
    {
        return await _repository.GetAllAsync();
    }
    
    public async Task<EnrichedEvent?> GetEventByIdAsync(string id)
    {
        return await _repository.GetByIdAsync(id);
    }
    
    public async Task<EnrichedEvent> CreateEventAsync(EnrichedEvent eventData)
    {
        eventData.Id = Guid.NewGuid().ToString();
        eventData.CreatedAt = DateTime.UtcNow;
        eventData.UpdatedAt = DateTime.UtcNow;
        eventData.Status = EventStatus.Draft;
        
        return await _repository.CreateAsync(eventData);
    }
    
    public async Task<EnrichedEvent> UpdateEventAsync(string id, EnrichedEvent eventData)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Event with id {id} not found");
        }
        
        eventData.Id = id;
        eventData.CreatedAt = existing.CreatedAt;
        eventData.CreatedBy = existing.CreatedBy;
        eventData.UpdatedAt = DateTime.UtcNow;
        
        return await _repository.UpdateAsync(id, eventData);
    }
    
    public async Task DeleteEventAsync(string id)
    {
        await _repository.DeleteAsync(id);
    }
    
    public async Task<EnrichedEvent> EnrichEventAsync(string id, string? description, List<string>? images, Dictionary<string, string>? customProperties)
    {
        var eventData = await _repository.GetByIdAsync(id);
        if (eventData == null)
        {
            throw new KeyNotFoundException($"Event with id {id} not found");
        }
        
        if (!string.IsNullOrEmpty(description))
            eventData.EnrichedDescription = description;
        
        if (images != null && images.Any())
            eventData.ImageGallery = images;
        
        if (customProperties != null && customProperties.Any())
        {
            foreach (var prop in customProperties)
            {
                eventData.CustomProperties[prop.Key] = prop.Value;
            }
        }
        
        eventData.UpdatedAt = DateTime.UtcNow;
        
        return await _repository.UpdateAsync(id, eventData);
    }
    
    public async Task<EnrichedEvent> ImportFromMomentusAsync(string momentusId, bool autoEnrich)
    {
        var momentusEvent = await _momentusService.GetEventByIdAsync(momentusId);
        if (momentusEvent == null)
        {
            throw new KeyNotFoundException($"Momentus event with id {momentusId} not found");
        }
        
        var enrichedEvent = new EnrichedEvent
        {
            Id = Guid.NewGuid().ToString(),
            Name = momentusEvent.Name,
            Description = momentusEvent.Description,
            StartDate = momentusEvent.StartDate,
            EndDate = momentusEvent.EndDate,
            Location = momentusEvent.Location,
            Type = EventType.Other,
            Status = EventStatus.Draft,
            MomentusId = momentusId,
            LastSyncedFromMomentus = DateTime.UtcNow,
            IsBookable = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        if (autoEnrich && !string.IsNullOrEmpty(momentusEvent.Description))
        {
            enrichedEvent.EnrichedDescription = $"Imported from Momentus: {momentusEvent.Description}";
        }
        
        return await _repository.CreateAsync(enrichedEvent);
    }
}
