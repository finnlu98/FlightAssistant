using System.ComponentModel.DataAnnotations;

namespace flight_assistant_backend.Data.Models;

public class Flight
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
    public required DateTime ArrivalTime { get; set; }

    [Required]
    public required float Price {get; set;}

    [Required]
    public required int TotalDuration {get; set;}

    public int NumberLayovers {get; set;}

    public int LayoverDuration {get; set;}

    [Required]
    public required string SearchUrl {get; set;}

    [Required]
    public required bool HasTargetPrice {get; set;}

}