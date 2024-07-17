using GeoLocateX.Domain.Models;

namespace GeoLocateX.Services.Interfaces;

public interface IIPBaseClient
{
    Task<GeoIpResponse> Fetch(string ipAddress, CancellationToken cancellationToken);
}
