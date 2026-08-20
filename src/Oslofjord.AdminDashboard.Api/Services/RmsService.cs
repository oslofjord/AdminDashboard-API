namespace Oslofjord.AdminDashboard.Api.Services;

public interface IRmsService
{
    Task<IEnumerable<RmsRoom>> GetRoomsAsync();
    Task<RmsRoom?> GetRoomByIdAsync(string rmsId);
    Task<RoomAvailability> CheckAvailabilityAsync(string rmsId, DateTime startDate, DateTime endDate);
    Task<bool> CreateBookingAsync(CreateRmsBooking booking);
}

public class RmsService : IRmsService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RmsService> _logger;
    
    public RmsService(HttpClient httpClient, ILogger<RmsService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<IEnumerable<RmsRoom>> GetRoomsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/rooms");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<List<RmsRoom>>() ?? new List<RmsRoom>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching rooms from RMS API");
            throw;
        }
    }
    
    public async Task<RmsRoom?> GetRoomByIdAsync(string rmsId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/rooms/{rmsId}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<RmsRoom>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching room {RmsId} from RMS API", rmsId);
            throw;
        }
    }
    
    public async Task<RoomAvailability> CheckAvailabilityAsync(string rmsId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"/api/rooms/{rmsId}/availability?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<RoomAvailability>() 
                   ?? new RoomAvailability { RoomId = rmsId, IsAvailable = false };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking availability for room {RmsId}", rmsId);
            throw;
        }
    }
    
    public async Task<bool> CreateBookingAsync(CreateRmsBooking booking)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/bookings", booking);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking in RMS");
            throw;
        }
    }
}

public class RmsRoom
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
    public List<string>? Amenities { get; set; }
}

public class RoomAvailability
{
    public required string RoomId { get; set; }
    public bool IsAvailable { get; set; }
    public int AvailableCount { get; set; }
}

public class CreateRmsBooking
{
    public required string RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfRooms { get; set; }
    public required string GuestName { get; set; }
    public required string GuestEmail { get; set; }
}
