namespace GeoGaurd.API.Services;

public interface IIpAddressResolver
{
    string ResolveCallerIpAddress(HttpContext context);
}
