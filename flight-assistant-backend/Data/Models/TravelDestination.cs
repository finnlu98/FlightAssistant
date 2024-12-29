using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace flight_assistant_backend.Data.Models
{
    public class TravelDestionation
    {
        [ForeignKey("Category")]
        public required string Code3 { get; set; }
        public virtual Country? Country { get; set; }
        public required DateTime TravelDate { get; set; }
    }
}
