using System.Text.Json;
using GeoLocateX.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using GeoLocateX.Data;
using GeoLocateX.Domain.Interfaces;
using GeoLocateX.Domain.Models;
using GeoLocateX.Domain.Configuration;
using GeoLocateX.Data.Entities;

namespace GeoLocateX.Services;

public class GeoIpService : IGeoIpService
{
    private readonly ILogger<GeoIpService> _logger;
    private readonly HttpClient _httpClient;
    private readonly IpBaseConfig _ipBaseConfig;
    private readonly IGeoLocationRepository _repository;
    private readonly IQueueService _queueService;
    
    public GeoIpService(
        IHttpClientFactory httpClientFactory,
        ILogger<GeoIpService> logger,
        IOptions<IpBaseConfig> ipBaseConfig,
        IGeoLocationRepository repository,
        IQueueService queueService)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("GeoIpClient");
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _queueService = queueService ?? throw new ArgumentNullException(nameof(queueService));
        _ipBaseConfig = ipBaseConfig.Value ?? throw new ArgumentException(nameof(ipBaseConfig));
    }

    public async Task<GeoIpResponse> GetGeoIPAsync(string ipAddress, CancellationToken cancellationToken)
    {
        var geoIpResponse = new GeoIpResponse();

        var apiKey = _ipBaseConfig.ApiKey;
        var baseAddress = _ipBaseConfig.BaseAddress;

        var existingIpAddress = await _repository.GetBatchProcessResponseByIpAddressAsync(ipAddress, cancellationToken);

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
            geoIpResponse = await Fetch(ipAddress, cancellationToken);

            var batchProcessItemResponse = new BatchProcessItemResponse();
            batchProcessItemResponse.Id = Guid.NewGuid();
            batchProcessItemResponse.Latitude = geoIpResponse.Latitude ?? null;
            batchProcessItemResponse.Longitude = geoIpResponse.Longitude ?? null;
            batchProcessItemResponse.CountryName = geoIpResponse.CountryName ?? null;
            batchProcessItemResponse.CountryCode = geoIpResponse.CountryCode ?? null;
            batchProcessItemResponse.IpAddress = geoIpResponse.Ip;
            batchProcessItemResponse.Timezones = geoIpResponse.TimeZones ?? null;
            batchProcessItemResponse.CreatedAt = DateTime.UtcNow;

            await _repository.AddBatchProcessResponseAsync(batchProcessItemResponse, cancellationToken);
        }
        return geoIpResponse;
    }
    public async Task<string> StartBatchProcess(
        IEnumerable<string> ipAddresses, CancellationToken cancellationToken)
    {
        var batchGuid = Guid.NewGuid();

        var batchProcessItems = new List<BatchProcessItem>();

        var existingIpAddresses = await _repository.GetBatchProcessItemResponseByIpsAsync(ipAddresses, cancellationToken);
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
        
        await _repository.AddBatchProcessAsync(batchProcess, cancellationToken);

        return $"api/geoip/status/{batchProcess.Id}";
    }

    public async Task<string> GetBatchStatus(Guid batchId, CancellationToken cancellationToken)
    {
        var batch = await _repository.GetBatchProcessByIdAsync(batchId, cancellationToken);

        if (batch == null) 
        {
            return $"Cannot find a batch with id {batchId}";
        }

        if (batch.Status == BatchProcessStatus.Completed) 
        {
            return "Completed";
        }

        var batchProcessItems = await _repository.GetBatchProcessItemsAsync(batchId, cancellationToken);

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

    private async Task<GeoIpResponse> Fetch(string ipAddress, CancellationToken cancellationToken)
    {
        var geoIpResponse = new GeoIpResponse();

        var apiKey = _ipBaseConfig.ApiKey;
        var baseAddress = _ipBaseConfig.BaseAddress;

        using (var response = await _httpClient.GetAsync($"{baseAddress}/info?ip={ipAddress}&apikey={apiKey}", cancellationToken))
        {
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadFromJsonAsync<IPBaseResponse<GeoIP>>(
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });

            var responseData = responseJson?.Data;

            if (responseData != null)
            {
                geoIpResponse.Latitude = responseData.Location.Latitude;
                geoIpResponse.Longitude = responseData.Location.Longitude;
                geoIpResponse.CountryName = responseData.Location.Country.Name;
                geoIpResponse.CountryCode = responseData.Location.Country.Alpha3;
                geoIpResponse.Ip = responseData.Ip;
                geoIpResponse.TimeZones = responseData?.Location?.Country?.Timezones != null
                                          ? string.Join(",", responseData.Location.Country.Timezones)
                                          : string.Empty;
            }
            return geoIpResponse;
        }
    }
}