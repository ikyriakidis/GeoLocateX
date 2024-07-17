using GeoLocateX.Data;
using GeoLocateX.Data.Entities;
using GeoLocateX.Domain.Interfaces;
using GeoLocateX.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GeoLocateX.Services;

public class GeoIpBackgroundService : BackgroundService, IGeoIpBackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<GeoIpBackgroundService> _logger;
    private readonly IQueueService _queueService;

    public GeoIpBackgroundService(
        IServiceScopeFactory scopeFactory, 
        ILogger<GeoIpBackgroundService> logger, 
        IQueueService queueService)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _queueService = queueService ?? throw new ArgumentNullException(nameof(queueService));
    }

    public async Task LoadUnfinishedBatchProcessItems(CancellationToken cancellationToken)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IBatchProcessRepository>();

            var batches = await repository.GetBatchProcessByStatusAsync(BatchProcessStatus.Queued, cancellationToken);

            if (batches != null)
            {
                foreach (var batch in batches)
                {
                    foreach (var batchProcessItem in batch.BatchProcessItems)
                    {
                        _queueService.Enqueue(batchProcessItem);
                    }
                }
            }
        }
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await LoadUnfinishedBatchProcessItems(cancellationToken);
        await base.StartAsync(cancellationToken);
    }
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("GeoIpBackgroundService is starting.");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {

                    var geoIpService = scope.ServiceProvider.GetRequiredService<IGeoIpService>();
                    var batchProcessItemResponseRepository = scope.ServiceProvider.GetRequiredService<IBatchProcessItemResponseRepository>();
                    var batchProcessRepository = scope.ServiceProvider.GetRequiredService<IBatchProcessRepository>();
                    var batchProcessItemsRepository = scope.ServiceProvider.GetRequiredService<IBatchProcessItemRepository>();

                    var dateTimeNow = DateTime.UtcNow;

                    _queueService.TryDequeue(out BatchProcessItem batchProcessItem);

                    if (batchProcessItem != null)
                    {
                        _logger.LogInformation($"GeoIpBackgroundService is querying: {batchProcessItem.IpAddress}.");

                        var existingIpAddress = await batchProcessItemResponseRepository.GetBatchProcessResponseByIpAddressAsync(batchProcessItem.IpAddress, cancellationToken);

                        if (existingIpAddress == null) 
                        {
                            var geoIpResponse = await geoIpService.GetGeoIPAsync(batchProcessItem.IpAddress, cancellationToken);
                        }

                        batchProcessItem.Status = BatchProcessItemStatus.Completed;
                        batchProcessItem.ProcessedAt = dateTimeNow;

                        await batchProcessItemsRepository.UpdateBatchProcessItemAsync(batchProcessItem, cancellationToken);

                        var batchProcessItems = await batchProcessItemsRepository.GetBatchProcessItemsAsync(batchProcessItem.BatchProcessId, cancellationToken);

                        bool anyQueuedOrProcessing = batchProcessItems
                            .Any(item => item.Status == BatchProcessItemStatus.Queued || item.Status == BatchProcessItemStatus.Processing);

                        if (!anyQueuedOrProcessing)
                        {
                            var batchProcess = await batchProcessRepository.GetBatchProcessByIdAsync(batchProcessItem.BatchProcessId, cancellationToken);

                            batchProcess.Status = BatchProcessStatus.Completed;
                            batchProcess.EndTime = dateTimeNow;

                            await batchProcessRepository.UpdateBatchProcessAsync(batchProcess, cancellationToken);

                            await batchProcessItemsRepository.DeleteBatchProcessItemsByBatchIdAsync(batchProcess.Id, cancellationToken);
                        }

                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GeoIpBackgroundService is stopping due to cancellation.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GeoIpBackgroundService.");
            }
        }

        _logger.LogInformation("GeoIpBackgroundService is stopping.");
    }
}
