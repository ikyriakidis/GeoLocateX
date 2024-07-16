namespace GeoLocateX.Api.Models;

public class GeoIpResponseModel
{
    public string IpAddress { get; set; }
    public string CountryCode { get; set; }
    public string CountryName { get; set; }
    public IEnumerable<string> Timezones { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
