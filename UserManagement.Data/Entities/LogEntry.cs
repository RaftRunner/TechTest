using System;
using UserManagement.Models;

namespace UserManagement.Data.Entities;
public class LogEntry
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Message { get; set; } = "";
    public ActionType ActionTypeId { get; set; }
    public DateTime DateCreated { get; set; }
    public User? User { get; set; }

    public enum ActionType
    {
        Created = 1,
        Updated = 2,
        Viewed = 3,
        Deleted = 4,
    }
}
