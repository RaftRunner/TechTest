using System.Linq;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet]
    public ViewResult List(string? filter)
    {
        var users = _userService.GetAll();

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
    public ViewResult Add() => View("AddEditUser", new AddEditUserViewModel());

    [HttpPost("AddUser")]
    public IActionResult Add(AddEditUserViewModel model)
    {
        if (!ModelState.IsValid) return View("AddEditUser", model);

        _userService.Create(new User
        {
            Forename = model.Forename!,
            Surname = model.Surname!,
            Email = model.Email!,
            IsActive = model.IsActive,
            DateOfBirth = model.DateOfBirth
        });

        return RedirectToAction("List");
    }

    [HttpGet("ViewUserDetails/{id}")]
    public ViewResult View(long id)
    {
        var user = _userService.GetAll().FirstOrDefault(u => u.Id == id);
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

        return View("ViewUserDetails", model); 
    }


    [HttpGet("EditUserDetails/{id}")]
    public IActionResult Edit(long id)
    {
        var user = _userService.GetAll().FirstOrDefault(u => u.Id == id);
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
    public IActionResult Edit(AddEditUserViewModel model)
    {
        if (!ModelState.IsValid) return View("AddEditUser", model);

        var user = _userService.GetAll().FirstOrDefault(u => u.Id == model.Id);
        if (user == null) return View("NotFound");

        user.Forename = model.Forename!;
        user.Surname = model.Surname!;
        user.Email = model.Email!;
        user.IsActive = model.IsActive;
        user.DateOfBirth = model.DateOfBirth;

        _userService.Update(user);

        return RedirectToAction("List");
    }

    [HttpGet("DeleteUser/{id}")]
    public IActionResult Delete(long id)
    {
        var user = _userService.GetAll().FirstOrDefault(u => u.Id == id);
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

        return View("DeleteUser", model);
    }

    [HttpPost("DeleteUser/{id}"), ActionName("Delete")]
    public IActionResult DeleteConfirmed(long id)
    {
        var user = _userService.GetAll().FirstOrDefault(u => u.Id == id);
        if (user != null)
            _userService.Delete(user);

        return RedirectToAction("List");
    }
}
