namespace GeoGaurd.API.Models;

public sealed class BlockedCountry
{
    public required string CountryCode { get; init; }
    public required string CountryName { get; init; }
    public DateTimeOffset BlockedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}
