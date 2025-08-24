using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Data.Entities;
using static UserManagement.Data.Entities.LogEntry;

namespace UserManagement.Services.Interfaces;
public interface ILogService
{
    Task AddLogAsync(long userId, ActionType actionTypeId, string message);
    Task<IEnumerable<LogEntry>> GetAllLogsAsync();
    Task<IEnumerable<LogEntry>> GetLogsForUserAsync(long userId);
}
