using GeoLocateX.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoLocateX.Data;

public class BatchProcessItemRepository : IBatchProcessItemRepository
{
    private readonly ApplicationContext _context;

    public BatchProcessItemRepository(ApplicationContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<BatchProcessItem>> GetBatchProcessItemsAsync(Guid batchProcessId, CancellationToken cancellationToken)
    {
        return await _context.BatchProcessItems
            .Where(item => item.BatchProcessId == batchProcessId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateBatchProcessItemAsync(BatchProcessItem batchProcessItem, CancellationToken cancellationToken)
    {
        _context.BatchProcessItems.Update(batchProcessItem);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteBatchProcessItemsByBatchIdAsync(Guid batchProcessId, CancellationToken cancellationToken)
    {
        var itemsToDelete = _context.BatchProcessItems.Where(item => item.BatchProcessId == batchProcessId);
        _context.BatchProcessItems.RemoveRange(itemsToDelete);
        await _context.SaveChangesAsync(cancellationToken);
    }
}