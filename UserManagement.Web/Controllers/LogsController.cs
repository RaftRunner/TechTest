using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IActionResult> Index(string searchUser = "", string actionFilter = "", int page = 1, int pageSize = 3)
        {
            var logs = _dataContext.GetAll<LogEntry>()
                .Include(l => l.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchUser))
                logs = logs.Where(l => l.User != null && l.User.Forename.Contains(searchUser));

            if (!string.IsNullOrEmpty(actionFilter))
                logs = logs.Where(l => l.ActionTypeId.ToString().Contains(actionFilter));

            var totalLogs = await logs.CountAsync();

            var logList = await logs
                .OrderByDescending(l => l.DateCreated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalLogs = totalLogs;
            ViewBag.SearchUser = searchUser;
            ViewBag.ActionFilter = actionFilter;

            return View(logList);
        }



        public async Task<IActionResult> Details(long id)
        {
            var log = await _dataContext.GetAll<LogEntry>()
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (log == null) return NotFound();

            return View(log);
        }
    }
}
