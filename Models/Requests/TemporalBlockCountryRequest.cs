using System.ComponentModel.DataAnnotations;

namespace GeoGaurd.API.Models.Requests;

public sealed class TemporalBlockCountryRequest
{
    [Required]
    [StringLength(2, MinimumLength = 2)]
    public string CountryCode { get; init; } = string.Empty;

    [Range(1, 1440)]
    public int DurationMinutes { get; init; }
}
