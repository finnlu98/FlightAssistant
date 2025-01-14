namespace flight_assistant_backend.Api.Settings;

public class HomeAssistantSettings
{
    public bool SendNotifications {get; set;} = false;
    public string? Url {get; set;}
    public string? Token  {get; set;}
}