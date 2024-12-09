using System.ComponentModel.DataAnnotations;

namespace flight_assistant_backend.Data.Models
{
    public class Country
    {
        
        [Key]
        public required string Code3 { get; set; }

        public string? Code2 { get; set; }
        
        [Required]
        public required string Name { get; set; }

        [Required]
        public required bool Visited { get; set; } 



    }
}
