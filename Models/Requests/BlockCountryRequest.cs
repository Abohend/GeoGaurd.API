using System.ComponentModel.DataAnnotations;

namespace GeoGaurd.API.Models.Requests;

public sealed class BlockCountryRequest
{
    [Required]
    [StringLength(2, MinimumLength = 2)]
    public string CountryCode { get; init; } = string.Empty;
}
