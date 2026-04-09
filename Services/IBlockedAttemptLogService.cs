using GeoGaurd.API.Models;

namespace GeoGaurd.API.Services;

public interface IBlockedAttemptLogService
{
    void Add(BlockedAttemptLogEntry blockedAttemptLogEntry);
    PagedResult<BlockedAttemptLogEntry> GetPaged(int page, int pageSize);
}
