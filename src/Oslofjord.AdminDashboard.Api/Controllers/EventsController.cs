using Microsoft.AspNetCore.Mvc;
using Oslofjord.AdminDashboard.Contracts.Models;
using Oslofjord.AdminDashboard.Contracts.Dtos;
using Oslofjord.AdminDashboard.Api.Services;

namespace Oslofjord.AdminDashboard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly ICentralApiService _centralApiService;
    private readonly ILogger<EventsController> _logger;
    
    public EventsController(ICentralApiService centralApiService, ILogger<EventsController> logger)
    {
        _centralApiService = centralApiService;
        _logger = logger;
    }
    
    /// <summary>
    /// Get all events from Central API
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnrichedEvent>>> GetAllEvents()
    {
        try
        {
            var events = await _centralApiService.GetEventsAsync();
            return Ok(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all events from Central API");
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    /// Get event by ID from Central API
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EnrichedEvent>> GetEventById(string id)
    {
        try
        {
            var eventData = await _centralApiService.GetEventByIdAsync(id);
            if (eventData == null)
                return NotFound($"Event with id {id} not found");
            
            return Ok(eventData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching event {Id} from Central API", id);
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    /// Create a new event via Central API
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<EnrichedEvent>> CreateEvent([FromBody] CreateEventDto dto)
    {
        try
        {
            var created = await _centralApiService.CreateEventAsync(dto);
            return CreatedAtAction(nameof(GetEventById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event via Central API");
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    /// Update an existing event via Central API
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<EnrichedEvent>> UpdateEvent(string id, [FromBody] UpdateEventDto dto)
    {
        try
        {
            var updated = await _centralApiService.UpdateEventAsync(id, dto);
            return Ok(updated);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound($"Event with id {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event {Id} via Central API", id);
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    /// Delete an event via Central API
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEvent(string id)
    {
        try
        {
            await _centralApiService.DeleteEventAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event {Id} via Central API", id);
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    /// Enrich an event with additional data via Central API
    /// </summary>
    [HttpPost("{id}/enrich")]
    public async Task<ActionResult<EnrichedEvent>> EnrichEvent(string id, [FromBody] EnrichEventDto dto)
    {
        try
        {
            var enriched = await _centralApiService.EnrichEventAsync(id, dto);
            return Ok(enriched);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound($"Event with id {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enriching event {Id} via Central API", id);
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    /// Import event from Momentus via Central API
    /// </summary>
    [HttpPost("import")]
    public async Task<ActionResult<EnrichedEvent>> ImportFromMomentus([FromBody] ImportFromMomentusDto dto)
    {
        try
        {
            var imported = await _centralApiService.ImportFromMomentusAsync(dto);
            return CreatedAtAction(nameof(GetEventById), new { id = imported.Id }, imported);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound("Momentus event not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing event from Momentus via Central API");
            return StatusCode(500, "Internal server error");
        }
    }
}
