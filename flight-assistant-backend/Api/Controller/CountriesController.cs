using flight_assistant_backend.Api.Data;
using flight_assistant_backend.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace flight_assistant_backend.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public CountriesController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IEnumerable<Country> Get()
            {
                return [.. _context.Countries];
            }

        [HttpPost]
        public IActionResult Post([FromBody] Country newCountry)
        {
            // Check if the country already exists in the list
            if (_context.Countries.Any(c => c.Code3 == newCountry.Code3))
            {
                return Conflict($"Country with code {newCountry.Code3} already exists.");
            }

             _context.Countries.Add(newCountry);
            _context.SaveChanges();

            return CreatedAtAction(nameof(Get), newCountry);
        }

        [HttpDelete("{code3}")]
        public IActionResult Delete(string code3)
        {
            var country = _context.Countries.FirstOrDefault(c => c.Code3 == code3);

            if (country == null)
            {
                return NotFound($"Country with Code3 '{code3}' not found.");
            }

            _context.Countries.Remove(country);
            _context.SaveChanges();

            return NoContent();
        }


        [HttpPut("{code3}")]
        public IActionResult SetVisited(string code3, [FromBody] bool visited)
        {
            var country = _context.Countries.FirstOrDefault(c => c.Code3 == code3);

            if (country == null)
            {
                return NotFound($"Country with Code3 '{code3}' not found.");
            }

            country.Visited = visited;

            // Save changes to the database
            _context.Countries.Update(country);
            _context.SaveChanges();

            return Ok($"Country with Code3 '{code3}' has been updated to visited: {visited}.");
        }
    }
}
