using System.Collections.Concurrent;
using GeoGaurd.API.Models;

namespace GeoGaurd.API.Repositories;

public sealed class InMemoryCountryBlockRepository : ICountryBlockRepository
{
    private readonly ConcurrentDictionary<string, BlockedCountry> _permanentBlockedCountries = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, TemporalBlockedCountry> _temporalBlockedCountries = new(StringComparer.OrdinalIgnoreCase);

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

    public bool IsBlocked(string countryCode)
    {
        if (_permanentBlockedCountries.ContainsKey(countryCode))
        {
            return true;
        }

        if (_temporalBlockedCountries.TryGetValue(countryCode, out var temporalBlock) &&
            temporalBlock.ExpiresAtUtc > DateTimeOffset.UtcNow)
        {
            return true;
        }

        return false;
    }

    public bool TryAddTemporal(TemporalBlockedCountry temporalBlockedCountry)
    {
        return _temporalBlockedCountries.TryAdd(temporalBlockedCountry.CountryCode, temporalBlockedCountry);
    }

    public IReadOnlyCollection<TemporalBlockedCountry> GetTemporalCountries()
    {
        return _temporalBlockedCountries.Values.ToArray();
    }
    
    public bool TryRemoveTemporal(string countryCode)
    {
        return _temporalBlockedCountries.TryRemove(countryCode, out _);
    }

    public int RemoveExpiredTemporalBlocks(DateTimeOffset nowUtc)
    {
        var removedCount = 0;

        foreach (var item in _temporalBlockedCountries)
        {
            if (item.Value.ExpiresAtUtc <= nowUtc && _temporalBlockedCountries.TryRemove(item.Key, out _))
            {
                removedCount++;
            }
        }

        return removedCount;
    }
}
