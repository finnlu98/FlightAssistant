namespace flight_assistant_backend.Api.Settings;

public class QuerySettings
{
    public int QueryPerNDay { get; set; }
    public int MaxQueries { get; set; }
    public int deleteNOld { get; set; }
    public int QueryAtHour { get; set; }
    public string? ApiKey {get; set;}
}