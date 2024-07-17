using GeoLocateX.Data.Entities;

namespace GeoLocateX.Data;

public interface IBatchProcessRepository
{
    Task<BatchProcess> GetBatchProcessByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<BatchProcess>> GetBatchProcessByStatusAsync(BatchProcessStatus status, CancellationToken cancellationToken);
    Task AddBatchProcessAsync(BatchProcess batchProcess, CancellationToken cancellationToken);
    Task UpdateBatchProcessAsync(BatchProcess batchProcess, CancellationToken cancellationToken);
}
