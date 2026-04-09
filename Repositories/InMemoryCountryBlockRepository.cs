using System.Collections.Concurrent;
using GeoGaurd.API.Models;

namespace GeoGaurd.API.Repositories;

public sealed class InMemoryCountryBlockRepository : ICountryBlockRepository
{
    private readonly ConcurrentDictionary<string, BlockedCountry> _permanentBlockedCountries = new(StringComparer.OrdinalIgnoreCase);
    public bool TryAddPermanent(BlockedCountry country)
    {
        return _permanentBlockedCountries.TryAdd(country.CountryCode, country);
    }

    public bool TryRemovePermanent(string countryCode)
    {
        return _permanentBlockedCountries.TryRemove(countryCode, out _);
    }

    public IReadOnlyCollection<BlockedCountry> GetPermanentCountries()
    {
        return _permanentBlockedCountries.Values.ToArray();
    }

}
