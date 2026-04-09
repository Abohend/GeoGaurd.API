using GeoGaurd.API.Models;

namespace GeoGaurd.API.Repositories;

public interface ICountryBlockRepository
{
    bool TryAddPermanent(BlockedCountry country);
    bool TryRemovePermanent(string countryCode);
    IReadOnlyCollection<BlockedCountry> GetPermanentCountries();
    bool IsBlocked(string countryCode);

    bool TryAddTemporal(TemporalBlockedCountry temporalBlockedCountry);
    IReadOnlyCollection<TemporalBlockedCountry> GetTemporalCountries();
    bool TryRemoveTemporal(string countryCode);
    int RemoveExpiredTemporalBlocks(DateTimeOffset nowUtc);
}
