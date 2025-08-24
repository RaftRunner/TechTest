using System;
using System.ComponentModel.DataAnnotations;
using UserManagement.Data.Entities;
using UserManagement.Models;

namespace UserManagement.Web.Models.Users;

public class AddEditUserViewModel
{
    public long Id { get; set; }

    [Required]
    public string? Forename { get; set; }

    [Required]
    public string? Surname { get; set; }

    [Required, EmailAddress]
    public string? Email { get; set; }

    public bool IsActive { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateOnly? DateOfBirth { get; set; }

    public User? User { get; set; }
    public IEnumerable<LogEntry> Logs { get; set; } = new List<LogEntry>();
}
