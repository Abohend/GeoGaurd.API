namespace GeoGaurd.API.Models;

public sealed class IpLookupResult
{
    public required string IpAddress { get; init; }
    public required string CountryCode { get; init; }
    public required string CountryName { get; init; }
    public string? Isp { get; init; }
}
