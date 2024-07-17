using GeoLocateX.Data.Entities;

namespace GeoLocateX.Data;

public interface IBatchProcessItemRepository
{
    Task<IEnumerable<BatchProcessItem>> GetBatchProcessItemsAsync(Guid batchProcessId, CancellationToken cancellationToken);
    Task UpdateBatchProcessItemAsync(BatchProcessItem batchProcessItem, CancellationToken cancellationToken);
    Task DeleteBatchProcessItemsByBatchIdAsync(Guid batchProcessId, CancellationToken cancellationToken);
}
