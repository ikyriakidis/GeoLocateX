namespace GeoLocateX.Domain.Models;

public class Country
{
    public string Alpha2 { get; set; }
    public string Alpha3 { get; set; }
    public List<string> CallingCodes { get; set; }
    public string Emoji { get; set; }
    public string Ioc { get; set; }
    public List<Language> Languages { get; set; }
    public string Name { get; set; }
    public string NameTranslated { get; set; }
    public List<string> Timezones { get; set; }
    public bool IsInEuropeanUnion { get; set; }
    public string Fips { get; set; }
    public int? GeonamesId { get; set; }
    public string HascId { get; set; }
    public string WikidataId { get; set; }
}

