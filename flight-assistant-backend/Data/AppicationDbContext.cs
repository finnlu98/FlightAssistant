using flight_assistant_backend.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace flight_assistant_backend.Api.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public required DbSet<Country> Countries { get; set; }

        public required DbSet<TravelDestionation> TravelDestinations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TravelDestionation>()
                .HasKey(td => new { td.Code3, td.TravelDate });

            modelBuilder.Entity<TravelDestionation>()
                .Property(td => td.TravelDate)
                .HasColumnType("timestamp");

            modelBuilder.Entity<TravelDestionation>()
                .HasOne(td => td.Country)       
                .WithMany()                          
                .HasForeignKey(td => td.Code3)        
                .HasPrincipalKey(cv => cv.Code3 );
        }

    }

}
