using flight_assistant_backend.Api.Controller;
using flight_assistant_backend.Api.Service;

public class FlightDataScheduler : BackgroundService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<FlightDataScheduler> _logger;
    private Timer? _timer;

    public FlightDataScheduler(IServiceProvider serviceProvider, ILogger<FlightDataScheduler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
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
        var scheduledTime = DateTime.Today.AddHours(12);

        if (now > scheduledTime)
        {
            scheduledTime = scheduledTime.AddDays(1);
        }

        var initialDelay = scheduledTime - now;

        _timer = new Timer(async _ => await RunTask(), null, initialDelay, TimeSpan.FromDays(1));
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

                _logger.LogInformation(flightData?.ToString() ?? "No flight data available.");
                _logger.LogInformation("Flight data task executed at: {Time}", DateTime.Now);
            }

            // Calculate and log the next scheduled time
            var nextScheduledTime = DateTime.Now.Date.AddDays(1).AddHours(14).AddMinutes(30);
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
