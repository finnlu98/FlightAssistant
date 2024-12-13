using flight_assistant_backend.Api.Data;
using flight_assistant_backend.Data.Models;
using flight_assistant_backend.Hubs;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace flight_assistant_backend.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly IHubContext<MapHub> _hubContext;

        public CountriesController(ApplicationDbContext context, IHubContext<MapHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IEnumerable<Country> Get()
        {
            return [.. _context.Countries];
        }

        [HttpPost]
        public IActionResult Post([FromBody] Country newCountry)
        {
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
        public async Task<IActionResult> SetVisited(string code3, [FromBody] bool visited)
        {
            var country = _context.Countries.FirstOrDefault(c => c.Code3 == code3);

            if (country == null)
            {
                return NotFound($"Country with Code3 '{code3}' not found.");
            }

            country.Visited = visited;

            _context.Countries.Update(country);
            _context.SaveChanges();

            await _hubContext.Clients.All.SendAsync("CountryUpdated", new { code3, visited });

            return Ok($"Country with Code3 '{code3}' has been updated to visited: {visited}.");
        }
    }
}
