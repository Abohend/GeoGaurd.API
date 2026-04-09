namespace GeoGaurd.API.Models;

public sealed class BlockedCountryDetail
{
    public required string CountryCode { get; init; }
    public required string CountryName { get; init; }
    public required DateTimeOffset BlockedAtUtc { get; init; }
    public DateTimeOffset? ExpiresAtUtc { get; init; }
    public required bool IsTemporal { get; init; }
}
