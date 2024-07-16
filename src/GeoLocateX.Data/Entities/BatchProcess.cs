namespace GeoLocateX.Data.Entities;

public class BatchProcess
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public BatchProcessStatus Status { get; set; }
    public ICollection<BatchProcessItem> BatchProcessItems { get; set; }
}

public enum BatchProcessStatus
{
    Queued,
    Running,
    Completed
}

