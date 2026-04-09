using GeoGaurd.API.Models;

namespace GeoGaurd.API.Services;

public interface ICountryBlockService
{
    BlockedCountry AddPermanentBlock(string countryCode);
    void RemovePermanentBlock(string countryCode);
    PagedResult<BlockedCountry> GetBlockedCountries(int page, int pageSize, string? search);
}
