using GeoLocateX.Data.Entities;
using GeoLocateX.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoLocateX.Data;

public class GeoLocationRepository : IGeoLocationRepository
{
    private readonly ApplicationContext _context;

    public GeoLocationRepository(ApplicationContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<BatchProcess> GetBatchProcessByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.BatchProcesses.FindAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<BatchProcess>> GetBatchProcessByStatusAsync(
        BatchProcessStatus status, CancellationToken cancellationToken)
    {
        return await _context
        .BatchProcesses
        .Include(item => item.BatchProcessItems)
        .Where(batch => batch.Status == status)
        .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BatchProcessItem>> GetBatchProcessItemsAsync(
        Guid batchProcessId, CancellationToken cancellationToken)
    {
        return await _context.BatchProcessItems
                             .Where(item => item.BatchProcessId == batchProcessId)
                             .ToListAsync(cancellationToken);
    }

    public async Task AddBatchProcessAsync(
        BatchProcess batchProcess, CancellationToken cancellationToken)
    {
        await _context.BatchProcesses.AddAsync(batchProcess, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateBatchProcessItemAsync(
        BatchProcessItem batchProcessItem, CancellationToken cancellationToken)
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

    public async Task UpdateBatchProcessAsync(
        BatchProcess batchProcess, CancellationToken cancellationToken)
    {
        _context.BatchProcesses.Update(batchProcess);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddBatchProcessResponseAsync(
    BatchProcessItemResponse batchProcessItemResponse, CancellationToken cancellationToken)
    {
        _context.Add(batchProcessItemResponse);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<BatchProcessItemResponse> GetBatchProcessResponseByIpAddressAsync(string ipAddress, CancellationToken cancellationToken)
    {
        return await _context.BatchProcessItemResponses.FirstOrDefaultAsync(item => item.IpAddress == ipAddress, cancellationToken);
    }

    public async Task<IEnumerable<BatchProcessItemResponse>> GetBatchProcessItemResponseByIpsAsync(
        IEnumerable<string> ipAddresses, CancellationToken cancellationToken)
    {
        return await _context.BatchProcessItemResponses.Where(item => ipAddresses.Contains(item.IpAddress)).ToListAsync(cancellationToken);
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
