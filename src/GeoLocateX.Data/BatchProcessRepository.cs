using GeoLocateX.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoLocateX.Data;

public class BatchProcessRepository : IBatchProcessRepository
{
    private readonly ApplicationContext _context;

    public BatchProcessRepository(ApplicationContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<BatchProcess> GetBatchProcessByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.BatchProcesses.FindAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<BatchProcess>> GetBatchProcessByStatusAsync(BatchProcessStatus status, CancellationToken cancellationToken)
    {
        return await _context.BatchProcesses
            .Include(item => item.BatchProcessItems)
            .Where(batch => batch.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task AddBatchProcessAsync(BatchProcess batchProcess, CancellationToken cancellationToken)
    {
        await _context.BatchProcesses.AddAsync(batchProcess, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateBatchProcessAsync(BatchProcess batchProcess, CancellationToken cancellationToken)
    {
        _context.BatchProcesses.Update(batchProcess);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
