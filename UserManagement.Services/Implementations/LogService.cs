using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public void AddLog(long userId, LogEntry.ActionType actionTypeId, string message)
    {
        var logEntry = new LogEntry()
        {
            UserId = userId,
            ActionTypeId = actionTypeId,
            Message = message,
            DateCreated = DateTime.Now,
        };

        _dataAccess.Create(logEntry);
    }

    public IEnumerable<LogEntry> GetAllLogs()
    {
        return _dataAccess.GetAllLogs();
    }
    public IEnumerable<LogEntry> GetLogsForUser(long userId)
    {
        return _dataAccess.GetAll<LogEntry>()
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.DateCreated)
            .ToList();
    }
}
