using GeoGaurd.API.Models;

namespace GeoGaurd.API.Services;

public interface IGeoLocationService
{
    Task<IpLookupResult> LookupAsync(string ipAddress, CancellationToken cancellationToken = default);
}
