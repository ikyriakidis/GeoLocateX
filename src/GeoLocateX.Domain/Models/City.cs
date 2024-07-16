
namespace GeoLocateX.Domain.Models;

public class City
{
    public string Fips { get; set; }
    public string Alpha2 { get; set; }
    public int? GeonamesId { get; set; }
    public string HascId { get; set; }
    public string WikidataId { get; set; }
    public string Name { get; set; }
    public string NameTranslated { get; set; }
}
