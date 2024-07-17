using GeoLocateX.Domain.Entities;
using GeoLocateX.Data;
using GeoLocateX.Domain.Interfaces;
using GeoLocateX.Domain.Models;
using GeoLocateX.Data.Entities;
using GeoLocateX.Services.Interfaces;

namespace GeoLocateX.Services;

public class GeoIpService : IGeoIpService
{
    private readonly IBatchProcessItemRepository _batchProcessItemRepository;
    private readonly IBatchProcessRepository _batchProcessRepository;
    private readonly IBatchProcessItemResponseRepository _batchProcessItemResponseRepository;
    private readonly IQueueService _queueService;
    private readonly IIPBaseClient _iPBaseClient;

    public GeoIpService(
        IBatchProcessItemRepository batchProcessItemRepository,
        IBatchProcessRepository batchProcessRepository,
        IBatchProcessItemResponseRepository batchProcessItemResponseRepository,
        IQueueService queueService,
        IIPBaseClient iPBaseClient)
    {
        _batchProcessItemRepository = batchProcessItemRepository ?? throw new ArgumentNullException(nameof(batchProcessItemRepository));
        _batchProcessRepository = batchProcessRepository ?? throw new ArgumentNullException(nameof(batchProcessRepository));
        _batchProcessItemResponseRepository = batchProcessItemResponseRepository ?? throw new ArgumentNullException(nameof(batchProcessItemResponseRepository));
        _queueService = queueService ?? throw new ArgumentNullException(nameof(queueService));
        _iPBaseClient = iPBaseClient ?? throw new ArgumentNullException(nameof(iPBaseClient));
    }

    public async Task<GeoIpResponse> GetGeoIPAsync(string ipAddress, CancellationToken cancellationToken)
    {
        var geoIpResponse = new GeoIpResponse();

        var existingIpAddress = await _batchProcessItemResponseRepository.GetBatchProcessResponseByIpAddressAsync(ipAddress, cancellationToken);

        if (existingIpAddress != null)
        {
            geoIpResponse.Ip = ipAddress;
            geoIpResponse.CountryName = existingIpAddress.CountryName;
            geoIpResponse.Latitude = existingIpAddress.Latitude;
            geoIpResponse.Longitude = existingIpAddress.Longitude;
            geoIpResponse.CountryCode = existingIpAddress.CountryCode;
            geoIpResponse.TimeZones = existingIpAddress.Timezones;
        }
        else 
        {
            geoIpResponse = await _iPBaseClient.Fetch(ipAddress, cancellationToken);

            var batchProcessItemResponse = new BatchProcessItemResponse();
            batchProcessItemResponse.Id = Guid.NewGuid();
            batchProcessItemResponse.Latitude = geoIpResponse.Latitude ?? null;
            batchProcessItemResponse.Longitude = geoIpResponse.Longitude ?? null;
            batchProcessItemResponse.CountryName = geoIpResponse.CountryName ?? null;
            batchProcessItemResponse.CountryCode = geoIpResponse.CountryCode ?? null;
            batchProcessItemResponse.IpAddress = geoIpResponse.Ip;
            batchProcessItemResponse.Timezones = geoIpResponse.TimeZones ?? null;
            batchProcessItemResponse.CreatedAt = DateTime.UtcNow;

            await _batchProcessItemResponseRepository.AddBatchProcessResponseAsync(batchProcessItemResponse, cancellationToken);
        }
        return geoIpResponse;
    }
    public async Task<string> StartBatchProcess(
        IEnumerable<string> ipAddresses, CancellationToken cancellationToken)
    {
        var batchGuid = Guid.NewGuid();

        var batchProcessItems = new List<BatchProcessItem>();

        var existingIpAddresses = await _batchProcessItemResponseRepository.GetBatchProcessItemResponseByIpsAsync(ipAddresses, cancellationToken);
        var filteredIpAddresses = ipAddresses.Where(x => !existingIpAddresses.Any(y => y.IpAddress == x));

        foreach (var ipAddress in filteredIpAddresses)
        {
            var batchProcessItem = new BatchProcessItem();
            batchProcessItem.Id = Guid.NewGuid();
            batchProcessItem.BatchProcessId = batchGuid;
            batchProcessItem.IpAddress = ipAddress;
            batchProcessItem.Status = BatchProcessItemStatus.Queued;

            batchProcessItems.Add(batchProcessItem);

            _queueService.Enqueue(batchProcessItem);
        }

        var status = batchProcessItems.Count() == 0 ? BatchProcessStatus.Completed : BatchProcessStatus.Queued;
        DateTime? endTime = batchProcessItems.Count() == 0 ? DateTime.UtcNow : null;

        var batchProcess = new BatchProcess
        {
            StartTime = DateTime.UtcNow,
            Id = batchGuid,
            Status = status,
            BatchProcessItems = batchProcessItems,
            EndTime = endTime,
        };
        
        await _batchProcessRepository.AddBatchProcessAsync(batchProcess, cancellationToken);

        return $"api/geoip/status/{batchProcess.Id}";
    }

    public async Task<string> GetBatchStatus(Guid batchId, CancellationToken cancellationToken)
    {
        var batch = await _batchProcessRepository.GetBatchProcessByIdAsync(batchId, cancellationToken);

        if (batch == null) 
        {
            return $"Cannot find a batch with id {batchId}";
        }

        if (batch.Status == BatchProcessStatus.Completed) 
        {
            return "Completed";
        }

        var batchProcessItems = await _batchProcessItemRepository.GetBatchProcessItemsAsync(batchId, cancellationToken);

        var incompleteCount = batchProcessItems
            .Count(item => 
            item.Status == BatchProcessItemStatus.Processing || 
            item.Status == BatchProcessItemStatus.Queued);

        var completedItems = batchProcessItems
            .Where(item => item.Status == BatchProcessItemStatus.Completed)
            .ToList();

        if (!completedItems.Any())
        {
            return "Insufficient data to estimate.";
        }

        // Calculate the total time spent on completed items
        var totalCompletedTime = completedItems.Sum(item =>
            (item.ProcessedAt.GetValueOrDefault() - batch.StartTime).TotalSeconds);

        // Calculate the average time spent per completed item
        var averageTimePerItem = totalCompletedTime / completedItems.Count;

        // Estimate the remaining time based on the average time per item
        var estimatedRemainingTime = averageTimePerItem * incompleteCount;

        // Format the estimated time as a human-readable string
        var estimatedTimeSpan = TimeSpan.FromSeconds(estimatedRemainingTime);

        return $"Estimated completion time in seconds: {estimatedTimeSpan}";
    }
}