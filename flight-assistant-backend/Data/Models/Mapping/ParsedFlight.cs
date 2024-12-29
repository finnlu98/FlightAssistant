
using flight_assistant_backend.Data.Models;

public class ParsedFlight
{
    public required Flight Flight {get; set;}
    public List<Layover>? Layovers {get; set;}
}