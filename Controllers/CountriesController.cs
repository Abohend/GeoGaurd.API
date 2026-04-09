using GeoGaurd.API.Models.Common;
using GeoGaurd.API.Models.Requests;
using GeoGaurd.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeoGaurd.API.Controllers;

[ApiController]
[Route("api/countries")]
public sealed class CountriesController : ControllerBase
{
    private readonly ICountryBlockService _countryBlockService;

    public CountriesController(ICountryBlockService countryBlockService)
    {
        _countryBlockService = countryBlockService;
    }

    [HttpPost("block")]
    public IActionResult BlockCountry([FromBody] BlockCountryRequest request)
    {
        var result = _countryBlockService.AddPermanentBlock(request.CountryCode);
        return CreatedAtAction(nameof(GetBlockedCountries), new { page = 1, pageSize = 10 }, ApiResponse.Ok(result, "Country blocked successfully."));
    }

    [HttpDelete("block/{countryCode}")]
    public IActionResult UnblockCountry(string countryCode)
    {
        _countryBlockService.RemovePermanentBlock(countryCode);
        return Ok(ApiResponse.Ok("Country unblocked successfully."));
    }

    [HttpGet("blocked")]
    public IActionResult GetBlockedCountries([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest(ApiResponse.Fail(400, "page and pageSize must be greater than 0."));
        }

        var result = _countryBlockService.GetBlockedCountries(page, pageSize, search);
        return Ok(ApiResponse.Ok(result));
    }
}
