using System;
using System.Collections.Generic;
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

    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    public IEnumerable<User> FilterByActive(bool isActive)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<User> GetAll() => _dataAccess.GetAll<User>();

    public void Create(User user)
    {
        _logService.AddLog(user.Id, ActionType.Created, $"User {user.Id} Created");
        _dataAccess.Create(user);
    }

    public void Update(User user)
    {
        _logService.AddLog(user.Id, ActionType.Updated, $"User {user.Id} Updated");
        _dataAccess.Update(user);
    }

    public void Delete(User user)
    {
        _logService.AddLog(user.Id, ActionType.Deleted, $"User {user.Id} Deleted");
        _dataAccess.Delete(user);
    }
}
