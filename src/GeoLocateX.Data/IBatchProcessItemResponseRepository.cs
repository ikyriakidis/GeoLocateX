using GeoLocateX.Domain.Entities;

namespace GeoLocateX.Data;

public interface IBatchProcessItemResponseRepository
{
    Task AddBatchProcessResponseAsync(BatchProcessItemResponse batchProcessItemResponse, CancellationToken cancellationToken);
    Task<BatchProcessItemResponse> GetBatchProcessResponseByIpAddressAsync(string ipAddress, CancellationToken cancellationToken);
    Task<IEnumerable<BatchProcessItemResponse>> GetBatchProcessItemResponseByIpsAsync(IEnumerable<string> ipAddresses, CancellationToken cancellationToken);
}
