using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace flight_assistant_backend.Data.Models;

public class Layover
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [ForeignKey("FlightId")] 
    [JsonIgnore]
    public virtual Flight Flight { get; set; } = null!;

    public Guid FlightId { get; set; }

    public string? Airport {get; set;}

    public string? Name {get; set;}

    public int? Duration {get; set;}
}

