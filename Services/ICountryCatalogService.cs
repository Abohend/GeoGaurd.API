namespace GeoGaurd.API.Services;

public interface ICountryCatalogService
{
    bool TryGetCountryName(string countryCode, out string countryName);
}
