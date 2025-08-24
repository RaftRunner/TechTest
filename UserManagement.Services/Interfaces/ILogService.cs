using System.Collections.Generic;
using UserManagement.Data.Entities;
using static UserManagement.Data.Entities.LogEntry;

namespace UserManagement.Services.Interfaces;
public interface ILogService
{
    public void AddLog(long userId, ActionType actionTypeId, string message);
    IEnumerable<LogEntry> GetAllLogs();
    IEnumerable<LogEntry> GetLogsForUser(long userId);


}
