namespace GeoGaurd.API.Models.Config;

public sealed class GeoLocationOptions
{
    public const string SectionName = "GeoLocationApi";

    public string BaseUrl { get; init; } = "https://ipapi.co";
    public string ApiKey { get; init; } = string.Empty;
}
