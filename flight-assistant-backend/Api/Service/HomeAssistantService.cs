using flight_assistant_backend.Api.Settings;
using Microsoft.Extensions.Options;


namespace flight_assistant_backend.Api.Service;

public class HomeAssistantService {
    
    private readonly HttpClient _httpClient;
    private readonly ILogger<HomeAssistantService> _logger;
    private readonly IOptions<HomeAssistantSettings> _homeAssistantSettings;

    
    public HomeAssistantService(
        HttpClient httpClient, 
        ILogger<HomeAssistantService> logger,
        IOptions<HomeAssistantSettings> homeAssistantSettings) {
        _httpClient = httpClient;
        _logger = logger;
        _homeAssistantSettings = homeAssistantSettings;
    }

    public async Task<bool> PublishFoundTargetPrice() {
        try {

            if(_homeAssistantSettings.Value.SendNotifications) {
                var url = $"{_homeAssistantSettings.Value.Url}/api/events/FOUNDTARGETPRICE";
                
                var request = new HttpRequestMessage(HttpMethod.Post, url);

                request.Headers.Add("Authorization", $"Bearer {_homeAssistantSettings.Value.Token}");

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode) {
                    _logger.LogInformation("Successfully sent found price.");
                    return true;
                }
                else {
                    _logger.LogError($"Failed to send found price to Home Assistant. Status code: {response.StatusCode}");
                    return false;
                }
            } else {
                _logger.LogInformation("Home Assistant not configured. No notification is sent");
                return false;
            }

        } catch (Exception ex) {
            _logger.LogError(ex, "An error occurred while trying to publish the target price.");
            return false;
        }
    }


}