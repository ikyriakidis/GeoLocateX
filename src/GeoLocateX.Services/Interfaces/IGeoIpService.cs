using GeoLocateX.Domain.Models;

namespace GeoLocateX.Domain.Interfaces;

public interface IGeoIpService
{
    Task<GeoIpResponse> GetGeoIPAsync(string ipAddress, CancellationToken cancellationToken);
    Task<string> StartBatchProcess(IEnumerable<string> ipAddresses, CancellationToken cancellationToken);
    Task<string> GetBatchStatus(Guid batchId, CancellationToken cancellationToken);
}