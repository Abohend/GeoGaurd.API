using GeoGaurd.API.Models;
using GeoGaurd.API.Repositories;

namespace GeoGaurd.API.Services;

public sealed class CountryBlockService : ICountryBlockService
{
    private readonly ICountryBlockRepository _countryBlockRepository;
    private readonly ICountryCatalogService _countryCatalogService;

    public CountryBlockService(ICountryBlockRepository countryBlockRepository, ICountryCatalogService countryCatalogService)
    {
        _countryBlockRepository = countryBlockRepository;
        _countryCatalogService = countryCatalogService;
    }

    public BlockedCountry AddPermanentBlock(string countryCode)
    {
        var cleanedCode = countryCode.Trim().ToUpperInvariant();
        if (!_countryCatalogService.TryGetCountryName(cleanedCode, out var countryName))
        {
            throw new ArgumentException("Invalid country code.");
        }

        var blockedCountry = new BlockedCountry
        {
            CountryCode = cleanedCode,
            CountryName = countryName
        };

        if (!_countryBlockRepository.TryAddPermanent(blockedCountry))
        {
            throw new InvalidOperationException("Country is already permanently blocked.");
        }

        // Overrides temporal block if it exists
        _countryBlockRepository.TryRemoveTemporal(cleanedCode);

        return blockedCountry;
    }

    public void RemovePermanentBlock(string countryCode)
    {
        var cleanedCode = countryCode.Trim().ToUpperInvariant();
        var permanentRemoved = _countryBlockRepository.TryRemovePermanent(cleanedCode);
        var temporalRemoved = _countryBlockRepository.TryRemoveTemporal(cleanedCode);

        if (!permanentRemoved && !temporalRemoved)
        {
            throw new KeyNotFoundException("Country is not currently blocked.");
        }
    }

    public PagedResult<BlockedCountryDetail> GetBlockedCountries(int page, int pageSize, string? search)
    {
        var now = DateTimeOffset.UtcNow;

        var permanent = _countryBlockRepository.GetPermanentCountries()
            .Select(c => new BlockedCountryDetail
            {
                CountryCode = c.CountryCode,
                CountryName = c.CountryName,
                BlockedAtUtc = c.BlockedAtUtc,
                IsTemporal = false
            });

        var temporal = _countryBlockRepository.GetTemporalCountries()
            .Where(c => c.ExpiresAtUtc > now)
            .Select(c => new BlockedCountryDetail
            {
                CountryCode = c.CountryCode,
                CountryName = c.CountryName,
                BlockedAtUtc = c.CreatedAtUtc,
                ExpiresAtUtc = c.ExpiresAtUtc,
                IsTemporal = true
            });

        var query = permanent.Concat(temporal);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.Trim();
            query = query.Where(item =>
                item.CountryCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                item.CountryName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        var ordered = query
            .OrderBy(item => item.CountryCode)
            .ToArray();

        var paged = ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArray();

        return new PagedResult<BlockedCountryDetail>
        {
            Items = paged,
            Page = page,
            PageSize = pageSize,
            TotalCount = ordered.Length
        };
    }

    public TemporalBlockedCountry AddTemporalBlock(string countryCode, int durationMinutes)
    {
        var cleanedCode = countryCode.Trim().ToUpperInvariant();
        if (!_countryCatalogService.TryGetCountryName(cleanedCode, out var countryName))
        {
            throw new ArgumentException("Invalid country code.");
        }

        if (_countryBlockRepository.IsBlocked(cleanedCode))
        {
            throw new InvalidOperationException("Country is already blocked.");
        }

        var block = new TemporalBlockedCountry
        {
            CountryCode = cleanedCode,
            CountryName = countryName,
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(durationMinutes)
        };

        if (!_countryBlockRepository.TryAddTemporal(block))
        {
            throw new InvalidOperationException("Country already has an active temporary block.");
        }

        return block;
    }

    public bool IsCountryBlocked(string countryCode)
    {
        return _countryBlockRepository.IsBlocked(countryCode.Trim().ToUpperInvariant());
    }

    public int RemoveExpiredTemporalBlocks()
    {
        return _countryBlockRepository.RemoveExpiredTemporalBlocks(DateTimeOffset.UtcNow);
    }
}
