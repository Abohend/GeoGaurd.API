using GeoGaurd.API.Services;

namespace GeoGaurd.API.BackgroundServices;

public sealed class TemporalBlockCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TemporalBlockCleanupService> _logger;

    public TemporalBlockCleanupService(IServiceProvider serviceProvider, ILogger<TemporalBlockCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var countryBlockService = scope.ServiceProvider.GetRequiredService<ICountryBlockService>();

            var removedCount = countryBlockService.RemoveExpiredTemporalBlocks();
            if (removedCount > 0)
            {
                _logger.LogInformation("Removed {Count} expired temporal block entries.", removedCount);
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
