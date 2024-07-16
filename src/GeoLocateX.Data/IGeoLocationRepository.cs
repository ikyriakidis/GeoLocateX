using GeoLocateX.Data.Entities;
using GeoLocateX.Domain.Entities;

namespace GeoLocateX.Data;

public interface IGeoLocationRepository
{
    Task<BatchProcess> GetBatchProcessByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<BatchProcess>> GetBatchProcessByStatusAsync(BatchProcessStatus status, CancellationToken cancellationToken);
    Task AddBatchProcessAsync(BatchProcess batchProcess, CancellationToken cancellationToken);
    Task UpdateBatchProcessAsync(BatchProcess batchProcess, CancellationToken cancellationToken);
    Task UpdateBatchProcessItemAsync(BatchProcessItem batchProcessItem, CancellationToken cancellationToken);
    Task<IEnumerable<BatchProcessItem>> GetBatchProcessItemsAsync(Guid batchProcessId, CancellationToken cancellationToken);
    Task DeleteBatchProcessItemsByBatchIdAsync(Guid batchProcessId, CancellationToken cancellationToken);
    Task<BatchProcessItemResponse> GetBatchProcessResponseByIpAddressAsync(string ipAddress, CancellationToken cancellationToken);
    Task AddBatchProcessResponseAsync(BatchProcessItemResponse batchProcessItemResponse, CancellationToken cancellationToken);
    Task<IEnumerable<BatchProcessItemResponse>> GetBatchProcessItemResponseByIpsAsync(IEnumerable<string> ipAddresses, CancellationToken cancellationToken);
    Task SaveAsync(CancellationToken cancellationToken);
}
