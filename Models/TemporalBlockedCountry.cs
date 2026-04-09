namespace GeoGaurd.API.Models;

public sealed class TemporalBlockedCountry
{
    public required string CountryCode { get; init; }
    public required string CountryName { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ExpiresAtUtc { get; init; }
}
