using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Data.Entities;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Implementations;
public class LogService : ILogService
{
    private readonly IDataContext _dataAccess;
    public LogService(IDataContext dataAccess)
    {
        _dataAccess = dataAccess;
    }

    public async Task AddLogAsync(long userId, LogEntry.ActionType actionTypeId, string message)
    {
        var logEntry = new LogEntry()
        {
            UserId = userId,
            ActionTypeId = actionTypeId,
            Message = message,
            DateCreated = DateTime.Now,
        };

        await _dataAccess.CreateAsync(logEntry);
    }

    public async Task<IEnumerable<LogEntry>> GetAllLogsAsync()
    {
        var logs = await _dataAccess.GetAllLogsAsync();
        return logs.ToList();
    }

    public async Task<IEnumerable<LogEntry>> GetLogsForUserAsync(long userId)
    {
        return await Task.FromResult(
            _dataAccess.GetAll<LogEntry>()
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.DateCreated)
                .ToList()
        );
    }
}
