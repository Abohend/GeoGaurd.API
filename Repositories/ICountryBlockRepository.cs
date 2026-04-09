using GeoGaurd.API.Models;

namespace GeoGaurd.API.Repositories;

public interface ICountryBlockRepository
{
    bool TryAddPermanent(BlockedCountry country);
    bool TryRemovePermanent(string countryCode);
    IReadOnlyCollection<BlockedCountry> GetPermanentCountries();
}
