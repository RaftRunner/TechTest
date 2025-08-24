using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data.Entities;
using static UserManagement.Data.Entities.LogEntry;

namespace UserManagement.Data;

public interface IDataContext
{
    IQueryable<TEntity> GetAll<TEntity>() where TEntity : class;

    Task CreateAsync<TEntity>(TEntity entity) where TEntity : class;
    Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class;
    Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class;
    Task AddViewAsync(long viewedUserId);

    Task AddLogAsync(long userId, ActionType actionTypeId, string message);
    Task<IQueryable<LogEntry>> GetAllLogsAsync();
}
