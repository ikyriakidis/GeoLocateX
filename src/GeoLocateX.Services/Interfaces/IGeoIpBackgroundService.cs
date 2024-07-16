namespace GeoLocateX.Services.Interfaces;

public interface IGeoIpBackgroundService
{
    Task LoadUnfinishedBatchProcessItems(CancellationToken cancellationToken);
}
