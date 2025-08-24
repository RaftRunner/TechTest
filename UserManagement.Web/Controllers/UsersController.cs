using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Interfaces;
using UserManagement.Web.Models.Users;
using static UserManagement.Data.Entities.LogEntry;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogService _logService;
    public UsersController(IUserService userService, ILogService logService)
    {
        _userService = userService;
        _logService = logService;
    }

    [HttpGet]
    public async Task<ViewResult> List(string? filter)
    {
        var users = await _userService.GetAllAsync();

        if (filter == "active")
            users = users.Where(u => u.IsActive);
        else if (filter == "nonactive")
            users = users.Where(u => !u.IsActive);

        var items = users.Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            IsActive = p.IsActive,
            DateOfBirth = p.DateOfBirth
        });

        var model = new UserListViewModel { Items = items.ToList() };
        return View(model);
    }

    [HttpGet("AddUser")]
    public ViewResult Add()
    {
        return View("AddEditUser", new AddEditUserViewModel());
    }
    [HttpPost("AddUser")]
    public async Task<IActionResult> Add(AddEditUserViewModel model)
    {
        if (!ModelState.IsValid) return View("AddEditUser", model);

        var user = new User
        {
            Forename = model.Forename!,
            Surname = model.Surname!,
            Email = model.Email!,
            IsActive = model.IsActive,
            DateOfBirth = model.DateOfBirth
        };

        await _userService.CreateAsync(user);

        await _logService.AddLogAsync(user.Id, ActionType.Created, "User created");

        return RedirectToAction("List");
    }

    [HttpGet("ViewUserDetails/{id}")]
    public async Task<ViewResult> View(long id)
    {
        var user = await _userService.ViewAsync(id);

        if (user == null)
            return View("NotFound");

        var model = new AddEditUserViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            IsActive = user.IsActive,
            DateOfBirth = user.DateOfBirth,
            Logs = await _logService.GetLogsForUserAsync(user.Id)
        };

        return View("ViewUserDetails", model);
    }



    [HttpGet("EditUserDetails/{id}")]
    public async Task<IActionResult> Edit(long id)
    {
        var users = await _userService.GetAllAsync();
        var user = users.FirstOrDefault(u => u.Id == id);
        if (user == null) return View("NotFound");

        var model = new AddEditUserViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            IsActive = user.IsActive,
            DateOfBirth = user.DateOfBirth
        };

        return View("AddEditUser", model);
    }

    [HttpPost("EditUserDetails/{id}")]
    public async Task<IActionResult> Edit(AddEditUserViewModel model)
    {
        if (!ModelState.IsValid) return View("AddEditUser", model);

        var users = await _userService.GetAllAsync();
        var user = users.FirstOrDefault(u => u.Id == model.Id);
        if (user == null) return View("NotFound");

        user.Forename = model.Forename!;
        user.Surname = model.Surname!;
        user.Email = model.Email!;
        user.IsActive = model.IsActive;
        user.DateOfBirth = model.DateOfBirth;

        await _userService.UpdateAsync(user);

        return RedirectToAction("List");
    }

    [HttpGet("DeleteUser/{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var users = await _userService.GetAllAsync();
        var user = users.FirstOrDefault(u => u.Id == id);
        if (user == null) return View("NotFound");
        await _logService.AddLogAsync(user.Id, ActionType.Viewed, "User viewed");

        var model = new AddEditUserViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            IsActive = user.IsActive,
            DateOfBirth = user.DateOfBirth,
            Logs = await _logService.GetLogsForUserAsync(user.Id) 
        };

        return View("DeleteUser", model);
    }

    [HttpPost("DeleteUser/{id}"), ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        var users = await _userService.GetAllAsync();
        var user = users.FirstOrDefault(u => u.Id == id);
        if (user != null)
            await _userService.DeleteAsync(user);

        return RedirectToAction("List");
    }
}
