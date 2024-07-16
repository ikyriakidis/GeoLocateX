namespace GeoLocateX.Domain.Models;

public class GeoIP
{
    public string Ip { get; set; }
    public string Hostname { get; set; }
    public string Type { get; set; }
    public Location Location { get; set; }
    public List<string> Tlds { get; set; }
}
