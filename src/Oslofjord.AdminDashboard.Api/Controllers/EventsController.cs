using Microsoft.AspNetCore.Mvc;
using Oslofjord.AdminDashboard.Contracts.Models;
using Oslofjord.AdminDashboard.Contracts.Dtos;
using Oslofjord.AdminDashboard.Api.Services;

namespace Oslofjord.AdminDashboard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly ILogger<EventsController> _logger;
    
    public EventsController(IEventService eventService, ILogger<EventsController> logger)
    {
        _eventService = eventService;
        _logger = logger;
    }
    
    /// <summary>
    /// Get all events
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnrichedEvent>>> GetAllEvents()
    {
        try
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all events");
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    /// Get event by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EnrichedEvent>> GetEventById(string id)
    {
        try
        {
            var eventData = await _eventService.GetEventByIdAsync(id);
            if (eventData == null)
                return NotFound($"Event with id {id} not found");
            
            return Ok(eventData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching event {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    /// Create a new event
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<EnrichedEvent>> CreateEvent([FromBody] CreateEventDto dto)
    {
        try
        {
            var eventData = new EnrichedEvent
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Location = dto.Location,
                ImageUrl = dto.ImageUrl,
                Type = (EventType)dto.Type,
                Status = EventStatus.Draft,
                IsBookable = dto.IsBookable,
                MaxParticipants = dto.MaxParticipants,
                BasePrice = dto.BasePrice,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            var created = await _eventService.CreateEventAsync(eventData);
            return CreatedAtAction(nameof(GetEventById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event");
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    /// Update an existing event
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<EnrichedEvent>> UpdateEvent(string id, [FromBody] UpdateEventDto dto)
    {
        try
        {
            var existing = await _eventService.GetEventByIdAsync(id);
            if (existing == null)
                return NotFound($"Event with id {id} not found");
            
            if (dto.Name != null) existing.Name = dto.Name;
            if (dto.Description != null) existing.Description = dto.Description;
            if (dto.StartDate.HasValue) existing.StartDate = dto.StartDate.Value;
            if (dto.EndDate.HasValue) existing.EndDate = dto.EndDate.Value;
            if (dto.Location != null) existing.Location = dto.Location;
            if (dto.ImageUrl != null) existing.ImageUrl = dto.ImageUrl;
            if (dto.Type.HasValue) existing.Type = (EventType)dto.Type.Value;
            if (dto.Status.HasValue) existing.Status = (EventStatus)dto.Status.Value;
            if (dto.IsBookable.HasValue) existing.IsBookable = dto.IsBookable.Value;
            if (dto.MaxParticipants.HasValue) existing.MaxParticipants = dto.MaxParticipants;
            if (dto.BasePrice.HasValue) existing.BasePrice = dto.BasePrice;
            
            var updated = await _eventService.UpdateEventAsync(id, existing);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Event with id {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    /// Delete an event
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEvent(string id)
    {
        try
        {
            await _eventService.DeleteEventAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    /// Enrich an event with additional data
    /// </summary>
    [HttpPost("{id}/enrich")]
    public async Task<ActionResult<EnrichedEvent>> EnrichEvent(string id, [FromBody] EnrichEventDto dto)
    {
        try
        {
            var enriched = await _eventService.EnrichEventAsync(
                id,
                dto.EnrichedDescription,
                dto.ImageGallery,
                dto.CustomProperties);
            
            return Ok(enriched);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Event with id {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enriching event {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    /// Import event from Momentus
    /// </summary>
    [HttpPost("import")]
    public async Task<ActionResult<EnrichedEvent>> ImportFromMomentus([FromBody] ImportFromMomentusDto dto)
    {
        try
        {
            var imported = await _eventService.ImportFromMomentusAsync(dto.MomentusId, dto.AutoEnrich);
            return CreatedAtAction(nameof(GetEventById), new { id = imported.Id }, imported);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing event from Momentus");
            return StatusCode(500, "Internal server error");
        }
    }
}
