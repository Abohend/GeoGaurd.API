namespace GeoGaurd.API.Models;

public sealed class BlockedAttemptLogEntry
{
    public required string IpAddress { get; init; }
    public required string CountryCode { get; init; }
    public required bool IsBlocked { get; init; }
    public required string UserAgent { get; init; }
    public DateTimeOffset TimestampUtc { get; init; } = DateTimeOffset.UtcNow;
}
