using flight_assistant_backend.Api.Controller;
using flight_assistant_backend.Api.Service;
using flight_assistant_backend.Api.Settings;
using Microsoft.Extensions.Options;

public class FlightDataScheduler : BackgroundService, IDisposable
{

    private readonly QuerySettings _querySettings;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<FlightDataScheduler> _logger;
    private Timer? _timer;

    public FlightDataScheduler(IServiceProvider serviceProvider, ILogger<FlightDataScheduler> logger, IOptions<QuerySettings> querySettings)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _querySettings = querySettings.Value;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Flight Scheduler is starting.");
        ScheduleTask();
        return Task.CompletedTask;
    }

    private void ScheduleTask()
    {

        var now = DateTime.Now;
        var scheduledTime = DateTime.Today.AddHours(_querySettings.QueryAtHour);

        if (now > scheduledTime)
        {
            scheduledTime = scheduledTime.AddDays(1);
        }

        var initialDelay = scheduledTime - now;

        _timer = new Timer(async _ => await RunTask(), null, initialDelay, TimeSpan.FromDays(_querySettings.QueryPerNDay));
        _logger.LogInformation("Get Flight data task scheduled at: {Time}", scheduledTime);
    }

    private async Task RunTask()
    {
        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var flightFinderService = scope.ServiceProvider.GetRequiredService<FlightFinderService>();
                var flightData = await flightFinderService.GetFlightData();

                if(flightData) {
                  _logger.LogInformation("Flight data collection succeded.");
                } else {
                    _logger.LogWarning("Flight data not succeeded");
                }
                
                _logger.LogInformation("Flight data task executed at: {Time}", DateTime.Now);
            }

            var nextScheduledTime = DateTime.Now.Date.AddDays(_querySettings.QueryPerNDay);
            _logger.LogInformation("Next task scheduled at: {Time}", nextScheduledTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while executing the flight data task.");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Flight Scheduler is stopping.");
        _timer?.Change(Timeout.Infinite, 0);
        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _timer?.Dispose();
    }
}
