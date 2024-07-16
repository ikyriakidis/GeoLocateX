using GeoLocateX.Data.Entities;

namespace GeoLocateX.Domain.Interfaces;

public interface IQueueService
{
    void Enqueue(BatchProcessItem queueItem);

    bool TryDequeue(out BatchProcessItem queueItem);
}