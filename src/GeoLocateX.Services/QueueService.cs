using GeoLocateX.Data.Entities;
using GeoLocateX.Domain.Interfaces;
using System.Collections.Concurrent;

namespace GeoLocateX.Services;

public class QueueService : IQueueService
{
    private readonly ConcurrentQueue<BatchProcessItem> _queue = new ConcurrentQueue<BatchProcessItem>();

    public void Enqueue(BatchProcessItem queueItem)
    {
        _queue.Enqueue(queueItem);
    }

    public bool TryDequeue(out BatchProcessItem queueItem)
    {
        return _queue.TryDequeue(out queueItem);
    }
}
