using GeoGaurd.API.Models;
using GeoGaurd.API.Repositories;

namespace GeoGaurd.API.Services;

public sealed class BlockedAttemptLogService : IBlockedAttemptLogService
{
    private readonly IBlockedAttemptsLogRepository _repository;

    public BlockedAttemptLogService(IBlockedAttemptsLogRepository repository)
    {
        _repository = repository;
    }

    public void Add(BlockedAttemptLogEntry blockedAttemptLogEntry)
    {
        _repository.Add(blockedAttemptLogEntry);
    }

    public PagedResult<BlockedAttemptLogEntry> GetPaged(int page, int pageSize)
    {
        var ordered = _repository.GetAll()
            .OrderByDescending(entry => entry.TimestampUtc)
            .ToArray();

        var paged = ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArray();

        return new PagedResult<BlockedAttemptLogEntry>
        {
            Items = paged,
            Page = page,
            PageSize = pageSize,
            TotalCount = ordered.Length
        };
    }
}
