using System.Collections.Concurrent;
using GeoGaurd.API.Models;

namespace GeoGaurd.API.Repositories;

public sealed class InMemoryBlockedAttemptsLogRepository : IBlockedAttemptsLogRepository
{
    private readonly ConcurrentQueue<BlockedAttemptLogEntry> _logEntries = new();

    public void Add(BlockedAttemptLogEntry blockedAttemptLogEntry)
    {
        _logEntries.Enqueue(blockedAttemptLogEntry);
    }

    public IReadOnlyCollection<BlockedAttemptLogEntry> GetAll()
    {
        return _logEntries.ToArray();
    }
}
