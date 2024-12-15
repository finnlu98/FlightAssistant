using flight_assistant_backend.Api.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace flight_assistant_backend.Data;

public class DatabaseInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(ApplicationDbContext context, ILogger<DatabaseInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        if (!await _context.Countries.AnyAsync())
        {
            try
            {
                var scriptPath = Path.Combine(AppContext.BaseDirectory, "Data/Initializer/initialize_countries.sql");
                string sqlScript = await File.ReadAllTextAsync(scriptPath);
                await _context.Database.ExecuteSqlRawAsync(sqlScript);
                _logger.LogInformation("No data in Countries found. Populating table with default values.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while populating the Countries table: {ex.Message}");
            }
        }
    }

    public void Initialize()
    {
        InitializeAsync().GetAwaiter().GetResult();
    }
}
