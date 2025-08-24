using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Interfaces;
using static UserManagement.Data.Entities.LogEntry;

namespace UserManagement.Services.Domain.Implementations;

public class UserService : IUserService
{
    private readonly IDataContext _dataAccess;
    private readonly ILogService _logService;
    public UserService(IDataContext dataAccess, ILogService logService)
    {
        _dataAccess = dataAccess;
        _logService = logService;
    }

    public async Task<IEnumerable<User>> FilterByActiveAsync(bool isActive)
    {
        return await Task.FromResult(
            _dataAccess.GetAll<User>().Where(u => u.IsActive == isActive).ToList()
        );
    }

    public async Task<IEnumerable<User>> GetAllAsync()
        => await Task.FromResult(_dataAccess.GetAll<User>().ToList());

    public async Task CreateAsync(User user)
    {
        await _logService.AddLogAsync(user.Id, ActionType.Created, $"User {user.Id} Created");
        await _dataAccess.CreateAsync(user);
    }

    public async Task UpdateAsync(User user)
    {
        await _logService.AddLogAsync(user.Id, ActionType.Updated, $"User {user.Id} Updated");
        await _dataAccess.UpdateAsync(user);
    }

    public async Task DeleteAsync(User user)
    {
        await _logService.AddLogAsync(user.Id, ActionType.Deleted, $"User {user.Id} Deleted");
        await _dataAccess.DeleteAsync(user);
    }

    public async Task<User?> ViewAsync(long userId)
    {
        var user = _dataAccess.GetAll<User>().FirstOrDefault(u => u.Id == userId);

        if (user != null)
        {
            await _dataAccess.AddViewAsync(user.Id);
        }

        return user;
    }
}
