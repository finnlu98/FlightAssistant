using System.ComponentModel.DataAnnotations;

namespace flight_assistant_backend.Data.Models;

public class FlightQuery
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public required string DepartureAirport { get; set; }
    
    [Required]
    public required string ArrivalAirport { get; set; }
    
    [Required]
    public required DateTime DepartureTime { get; set; }
    
    [Required]
    public required DateTime ReturnTime { get; set; }

    [Required]
    public required float TargetPrice {get; set;}
}

public class FlightQueryParse {
    public required string SearchUrl {get; set;}

    public required float TargetPrice {get; set;}
}