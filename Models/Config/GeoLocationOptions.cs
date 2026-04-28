namespace GeoGaurd.API.Models.Config;

public sealed class GeoLocationOptions
{
    public const string SectionName = "GeoLocationApi";

    public string BaseUrl { get; init; } = "https://ipapi.co";
    public string ApiKey { get; init; } = string.Empty;
    public ResilienceOptions Resilience { get; init; } = new();
}

public sealed class ResilienceOptions
{
    public int MaxRetryAttempts { get; init; } = 3;
    public double RetryBaseDelaySeconds { get; init; } = 2.0;
    public double CircuitBreakerDurationSeconds { get; init; } = 30.0;
    public double TotalTimeoutSeconds { get; init; } = 60.0;
}
