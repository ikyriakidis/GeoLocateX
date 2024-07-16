namespace GeoLocateX.Domain.Models;

public sealed class GeoIpResponse
{
    public string Ip { get; set; }
    public string? CountryCode { get; set; }
    public string? CountryName { get; set; }
    public string? TimeZones { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
