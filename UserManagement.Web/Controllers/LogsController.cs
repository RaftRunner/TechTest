using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Data.Entities;

namespace UserManagement.Web.Controllers
{
    public class LogsController : Controller
    {
        private readonly IDataContext _dataContext;

        public LogsController(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IActionResult Index(string searchUser = "", string actionFilter = "")
        {
            var logs = _dataContext.GetAll<LogEntry>().Include(l => l.User).AsQueryable();

            if (!string.IsNullOrEmpty(searchUser))
                logs = logs.Where(l => l.User != null && l.User.Forename.Contains(searchUser));

            if (!string.IsNullOrEmpty(actionFilter))
                logs = logs.Where(l => l.ActionTypeId.ToString().Contains(actionFilter));

            var logList = logs
                .OrderByDescending(l => l.DateCreated)
                .Take(100)
                .ToList();

            return View(logList); 
        }

        public IActionResult Details(long id)
        {
            var log = _dataContext.GetAll<LogEntry>()
                        .Include(l => l.User)
                        .FirstOrDefault(l => l.Id == id);

            if (log == null) return NotFound();

            return View(log);
        }
    }
}
