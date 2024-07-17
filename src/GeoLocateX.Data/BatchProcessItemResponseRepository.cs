using GeoLocateX.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoLocateX.Data;

public class BatchProcessItemResponseRepository : IBatchProcessItemResponseRepository
{
    private readonly ApplicationContext _context;

    public BatchProcessItemResponseRepository(ApplicationContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddBatchProcessResponseAsync(BatchProcessItemResponse batchProcessItemResponse, CancellationToken cancellationToken)
    {
        _context.Add(batchProcessItemResponse);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<BatchProcessItemResponse> GetBatchProcessResponseByIpAddressAsync(string ipAddress, CancellationToken cancellationToken)
    {
        return await _context.BatchProcessItemResponses.FirstOrDefaultAsync(item => item.IpAddress == ipAddress, cancellationToken);
    }

    public async Task<IEnumerable<BatchProcessItemResponse>> GetBatchProcessItemResponseByIpsAsync(IEnumerable<string> ipAddresses, CancellationToken cancellationToken)
    {
        return await _context.BatchProcessItemResponses
            .Where(item => ipAddresses.Contains(item.IpAddress))
            .ToListAsync(cancellationToken);
    }
}