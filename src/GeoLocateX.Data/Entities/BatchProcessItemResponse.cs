namespace GeoLocateX.Domain.Entities;

public sealed class BatchProcessItemResponse
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string IpAddress { get; set; }
    public string? CountryCode { get; set; }
    public string? CountryName { get; set; }
    public string? Timezones { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
