using System.Text.Json;
using System.Text.Json.Serialization;
using GeoGaurd.API.Models;
using GeoGaurd.API.Models.Config;
using Microsoft.Extensions.Options;

namespace GeoGaurd.API.Services;

public sealed class GeoLocationService : IGeoLocationService
{
    private readonly HttpClient _httpClient;
    private readonly GeoLocationOptions _options;

    public GeoLocationService(HttpClient httpClient, IOptions<GeoLocationOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<IpLookupResult> LookupAsync(string ipAddress, CancellationToken cancellationToken = default)
    {
        var requestUri = string.IsNullOrWhiteSpace(_options.ApiKey)
            ? $"{ipAddress}/json/"
            : $"{ipAddress}/json/?key={Uri.EscapeDataString(_options.ApiKey)}";

        using var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Geo-location provider returned status code {(int)response.StatusCode}.");
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var payload = await JsonSerializer.DeserializeAsync<IpApiResponse>(stream, cancellationToken: cancellationToken);

        if (payload is null || string.IsNullOrWhiteSpace(payload.CountryCode))
        {
            throw new InvalidOperationException("Geo-location provider returned incomplete country data.");
        }

        return new IpLookupResult
        {
            IpAddress = string.IsNullOrWhiteSpace(payload.Ip) ? ipAddress : payload.Ip,
            CountryCode = payload.CountryCode.ToUpperInvariant(),
            CountryName = payload.CountryName ?? "Unknown",
            Isp = payload.Org
        };
    }

    private sealed class IpApiResponse
    {
        [JsonPropertyName("ip")]
        public string? Ip { get; init; }

        [JsonPropertyName("country_code")]
        public string? CountryCode { get; init; }

        [JsonPropertyName("country_name")]
        public string? CountryName { get; init; }

        [JsonPropertyName("org")]
        public string? Org { get; init; }
    }
}
