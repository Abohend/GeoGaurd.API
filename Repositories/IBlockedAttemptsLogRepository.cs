using GeoGaurd.API.Models;

namespace GeoGaurd.API.Repositories;

public interface IBlockedAttemptsLogRepository
{
    void Add(BlockedAttemptLogEntry blockedAttemptLogEntry);
    IReadOnlyCollection<BlockedAttemptLogEntry> GetAll();
}
