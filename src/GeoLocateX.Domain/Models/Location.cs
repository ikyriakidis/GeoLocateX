namespace GeoLocateX.Domain.Models;

public class Location
{
    public int GeonamesId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Zip { get; set; }
    public Country Country { get; set; }
    public City City { get; set; }
}
