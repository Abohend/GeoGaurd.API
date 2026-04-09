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
        if (!_countryCatalogService.TryGetCountryName(countryCode, out var countryName))
        {
            throw new ArgumentException("Invalid country code.");
        }

        var blockedCountry = new BlockedCountry
        {
            CountryCode = countryCode.Trim().ToUpperInvariant(),
            CountryName = countryName
        };

        if (!_countryBlockRepository.TryAddPermanent(blockedCountry))
        {
            throw new InvalidOperationException("Country is already permanently blocked.");
        }

        return blockedCountry;
    }

    public void RemovePermanentBlock(string countryCode)
    {
        if (!_countryBlockRepository.TryRemovePermanent(countryCode.Trim().ToUpperInvariant()))
        {
            throw new KeyNotFoundException("Country is not currently blocked.");
        }
    }

    public PagedResult<BlockedCountry> GetBlockedCountries(int page, int pageSize, string? search)
    {
        var query = _countryBlockRepository.GetPermanentCountries().AsEnumerable();

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

        return new PagedResult<BlockedCountry>
        {
            Items = paged,
            Page = page,
            PageSize = pageSize,
            TotalCount = ordered.Length
        };
    }

}
