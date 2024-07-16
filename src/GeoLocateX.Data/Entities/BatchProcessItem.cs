namespace GeoLocateX.Data.Entities;

public class BatchProcessItem
{
    public Guid Id { get; set; }
    public Guid BatchProcessId { get; set; }
    public BatchProcess BatchProcess { get; set; }
    public string IpAddress { get; set; }
    public BatchProcessItemStatus Status { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

public enum BatchProcessItemStatus
{
    Queued,
    Processing,
    Completed
}
