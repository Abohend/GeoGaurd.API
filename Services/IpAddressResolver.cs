using System.Net;

namespace GeoGaurd.API.Services;

public sealed class IpAddressResolver : IIpAddressResolver
{
    public string ResolveCallerIpAddress(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            var first = forwardedFor.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(first))
            {
                return first;
            }
        }

        var remoteIp = context.Connection.RemoteIpAddress;
        if (remoteIp is null)
        {
            throw new InvalidOperationException("Unable to resolve caller IP address.");
        }

        if (IPAddress.IsLoopback(remoteIp))
        {
            return "8.8.8.8";
        }

        return remoteIp.ToString();
    }
}
