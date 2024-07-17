using System.Net.Http.Json;
using System.Text.Json;
using GeoLocateX.Domain.Configuration;
using GeoLocateX.Domain.Models;
using GeoLocateX.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace GeoLocateX.Services;

public class IPBaseClient : IIPBaseClient
{
    private readonly HttpClient _httpClient;
    private readonly IpBaseConfig _ipBaseConfig;

    public IPBaseClient(
        IHttpClientFactory httpClientFactory,
        IOptions<IpBaseConfig> ipBaseConfig)
    {
        _httpClient = httpClientFactory.CreateClient("GeoIpClient");
        _ipBaseConfig = ipBaseConfig.Value ?? throw new ArgumentException(nameof(ipBaseConfig));
    }

    public async Task<GeoIpResponse> Fetch(string ipAddress, CancellationToken cancellationToken)
    {
        var geoIpResponse = new GeoIpResponse();

        var apiKey = _ipBaseConfig.ApiKey;
        var baseAddress = _ipBaseConfig.BaseAddress;

        using (var response = await _httpClient.GetAsync($"{baseAddress}/info?ip={ipAddress}&apikey={apiKey}", cancellationToken))
        {
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadFromJsonAsync<IPBaseResponse<GeoIP>>(
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });

            var responseData = responseJson?.Data;

            if (responseData != null)
            {
                geoIpResponse.Latitude = responseData.Location.Latitude;
                geoIpResponse.Longitude = responseData.Location.Longitude;
                geoIpResponse.CountryName = responseData.Location.Country.Name;
                geoIpResponse.CountryCode = responseData.Location.Country.Alpha3;
                geoIpResponse.Ip = responseData.Ip;
                geoIpResponse.TimeZones = responseData?.Location?.Country?.Timezones != null
                                          ? string.Join(",", responseData.Location.Country.Timezones)
                                          : string.Empty;
            }
            return geoIpResponse;
        }
    }

}
