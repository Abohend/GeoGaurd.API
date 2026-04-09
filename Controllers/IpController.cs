using System.Net;
using GeoGaurd.API.Models;
using GeoGaurd.API.Models.Common;
using GeoGaurd.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeoGaurd.API.Controllers;

[ApiController]
[Route("api/ip")]
public sealed class IpController : ControllerBase
{
    private readonly IGeoLocationService _geoLocationService;
    private readonly IIpAddressResolver _ipAddressResolver;
    private readonly ICountryBlockService _countryBlockService;
    private readonly IBlockedAttemptLogService _blockedAttemptLogService;

    public IpController(
        IGeoLocationService geoLocationService,
        IIpAddressResolver ipAddressResolver,
        ICountryBlockService countryBlockService,
        IBlockedAttemptLogService blockedAttemptLogService)
    {
        _geoLocationService = geoLocationService;
        _ipAddressResolver = ipAddressResolver;
        _countryBlockService = countryBlockService;
        _blockedAttemptLogService = blockedAttemptLogService;
    }

    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup([FromQuery] string? ipAddress, CancellationToken cancellationToken)
    {
        var ipToLookup = string.IsNullOrWhiteSpace(ipAddress)
            ? _ipAddressResolver.ResolveCallerIpAddress(HttpContext)
            : ipAddress.Trim();

        if (!IPAddress.TryParse(ipToLookup, out _))
        {
            return BadRequest(ApiResponse.Fail(400, "Invalid IP format."));
        }

        var result = await _geoLocationService.LookupAsync(ipToLookup, cancellationToken);
        return Ok(ApiResponse.Ok(result));
    }

    [HttpGet("check-block")]
    public async Task<IActionResult> CheckBlock(CancellationToken cancellationToken)
    {
        var callerIp = _ipAddressResolver.ResolveCallerIpAddress(HttpContext);
        var lookupResult = await _geoLocationService.LookupAsync(callerIp, cancellationToken);
        var isBlocked = _countryBlockService.IsCountryBlocked(lookupResult.CountryCode);

        _blockedAttemptLogService.Add(new BlockedAttemptLogEntry
        {
            IpAddress = callerIp,
            CountryCode = lookupResult.CountryCode,
            IsBlocked = isBlocked,
            UserAgent = HttpContext.Request.Headers.UserAgent.ToString()
        });

        return Ok(ApiResponse.Ok(new
        {
            callerIp,
            lookupResult.CountryCode,
            lookupResult.CountryName,
            isBlocked
        }));
    }
}
