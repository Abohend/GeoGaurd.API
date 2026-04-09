using GeoGaurd.API.Models;

namespace GeoGaurd.API.Services;

public interface ICountryBlockService
{
    BlockedCountry AddPermanentBlock(string countryCode);
    void RemovePermanentBlock(string countryCode);
    PagedResult<BlockedCountryDetail> GetBlockedCountries(int page, int pageSize, string? search);
    TemporalBlockedCountry AddTemporalBlock(string countryCode, int durationMinutes);
    bool IsCountryBlocked(string countryCode);
    int RemoveExpiredTemporalBlocks();
}
