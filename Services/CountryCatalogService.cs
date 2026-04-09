using System.Globalization;

namespace GeoGaurd.API.Services;

public sealed class CountryCatalogService : ICountryCatalogService
{
    private readonly Dictionary<string, string> _countriesByCode;

    public CountryCatalogService()
    {
        _countriesByCode = CultureInfo
            .GetCultures(CultureTypes.SpecificCultures)
            .Select(culture => new RegionInfo(culture.Name))
            .GroupBy(region => region.TwoLetterISORegionName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First().EnglishName, StringComparer.OrdinalIgnoreCase);
    }

    public bool TryGetCountryName(string countryCode, out string countryName)
    {
        var normalizedCode = countryCode.Trim().ToUpperInvariant();
        return _countriesByCode.TryGetValue(normalizedCode, out countryName!);
    }
}
