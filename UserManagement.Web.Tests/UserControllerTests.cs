using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserManagement.Data.Entities;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;

namespace UserManagement.Data.Tests
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _userService = new();
        private readonly Mock<ILogService> _logService = new();

        private UsersController CreateController() => new(_userService.Object, _logService.Object);

        private User[] SetupUsers()
        {
            var users = new[]
            {
                new User { Id = 1, Forename = "John", Surname = "Doe", Email = "jdoe@example.com", IsActive = true },
                new User { Id = 2, Forename = "Inactive", Surname = "User", Email = "iuser@example.com", IsActive = false }
            };

            _userService.Setup(s => s.GetAllAsync()).ReturnsAsync(users);
            return users;
        }

        [Fact]
        public async Task List_ShouldReturnAllUsers_WhenNoFilter()
        {
            var controller = CreateController();
            var users = SetupUsers();

            var result = await controller.List(null) as ViewResult;

            result!.Model
                .Should().BeOfType<UserListViewModel>()
                .Which.Items.Should().BeEquivalentTo(users.Select(u => new UserListItemViewModel
                {
                    Id = u.Id,
                    Forename = u.Forename,
                    Surname = u.Surname,
                    Email = u.Email,
                    IsActive = u.IsActive,
                    DateOfBirth = u.DateOfBirth
                }));
        }

        [Fact]
        public async Task List_ShouldReturnActiveUsers_WhenFilterActive()
        {
            var controller = CreateController();
            var users = SetupUsers();

            var result = await controller.List("active") as ViewResult;

            var activeUsers = users.Where(u => u.IsActive).ToList();
            result!.Model.Should().BeOfType<UserListViewModel>()
                .Which.Items.Should().BeEquivalentTo(activeUsers.Select(u => new UserListItemViewModel
                {
                    Id = u.Id,
                    Forename = u.Forename,
                    Surname = u.Surname,
                    Email = u.Email,
                    IsActive = u.IsActive,
                    DateOfBirth = u.DateOfBirth
                }));
        }

        [Fact]
        public async Task List_ShouldReturnNonActiveUsers_WhenFilterNonActive()
        {
            var controller = CreateController();
            var users = SetupUsers();

            var result = await controller.List("nonactive") as ViewResult;

            var nonActiveUsers = users.Where(u => !u.IsActive).ToList();
            result!.Model.Should().BeOfType<UserListViewModel>()
                .Which.Items.Should().BeEquivalentTo(nonActiveUsers.Select(u => new UserListItemViewModel
                {
                    Id = u.Id,
                    Forename = u.Forename,
                    Surname = u.Surname,
                    Email = u.Email,
                    IsActive = u.IsActive,
                    DateOfBirth = u.DateOfBirth
                }));
        }

        [Fact]
        public void Add_Get_ShouldReturnAddEditView()
        {
            var controller = CreateController();

            var result = controller.Add() as ViewResult;

            result!.ViewName.Should().Be("AddEditUser");
            result.Model.Should().BeOfType<AddEditUserViewModel>();
        }

        [Fact]
        public async Task Add_Post_ShouldCreateUser_AndRedirect()
        {
            var controller = CreateController();
            var model = new AddEditUserViewModel
            {
                Forename = "John",
                Surname = "Doe",
                Email = "jdoe@example.com",
                IsActive = true
            };

            var result = await controller.Add(model) as RedirectToActionResult;

            _userService.Verify(s => s.CreateAsync(It.Is<User>(u => u.Forename == "John" && u.Email == "jdoe@example.com")), Times.Once);
            _logService.Verify(l => l.AddLogAsync(It.IsAny<long>(), It.IsAny<LogEntry.ActionType>(), It.IsAny<string>()), Times.Once);
            result!.ActionName.Should().Be("List");
        }

        [Fact]
        public async Task View_ShouldReturnUserDetails_WhenUserExists()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            var mockLogService = new Mock<ILogService>();

            var user = new User
            {
                Id = 1,
                Forename = "John",
                Surname = "Doe",
                Email = "john.doe@example.com",
                IsActive = true,
                DateOfBirth = new DateOnly(2000, 1, 1)
            };

            mockUserService.Setup(s => s.ViewAsync(1))
                .ReturnsAsync(user);

            mockLogService.Setup(s => s.GetLogsForUserAsync(1))
                .ReturnsAsync(new List<LogEntry>());

            var controller = new UsersController(mockUserService.Object, mockLogService.Object);

            // Act
            var result = await controller.View(1) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result!.ViewName.Should().Be("ViewUserDetails");
            result.Model.Should().BeOfType<AddEditUserViewModel>()
                .Which.Id.Should().Be(1);
        }


        [Fact]
        public async Task Edit_Get_ShouldReturnEditView_WhenUserExists()
        {
            var controller = CreateController();
            var users = SetupUsers();

            var result = await controller.Edit(1) as ViewResult;

            result!.ViewName.Should().Be("AddEditUser");
            result.Model.Should().BeOfType<AddEditUserViewModel>()
                .Which.Id.Should().Be(1);
        }

        [Fact]
        public async Task Edit_Post_ShouldUpdateUser_AndRedirect()
        {
            var controller = CreateController();
            var users = SetupUsers();
            var model = new AddEditUserViewModel { Id = 1, Forename = "Updated", Surname = "User", Email = "updated@example.com" };

            var result = await controller.Edit(model) as RedirectToActionResult;

            _userService.Verify(s => s.UpdateAsync(It.Is<User>(u => u.Id == 1 && u.Forename == "Updated")), Times.Once);
            result!.ActionName.Should().Be("List");
        }

        [Fact]
        public async Task Delete_Get_ShouldReturnDeleteView_WithLogs()
        {
            var controller = CreateController();
            var users = SetupUsers();
            _logService.Setup(l => l.GetLogsForUserAsync(1)).ReturnsAsync(new List<LogEntry>());

            var result = await controller.Delete(1) as ViewResult;

            result!.ViewName.Should().Be("DeleteUser");
            result.Model.Should().BeOfType<AddEditUserViewModel>();
            _logService.Verify(l => l.AddLogAsync(1, LogEntry.ActionType.Viewed, "User viewed"), Times.Once);
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteUser_AndRedirect()
        {
            var controller = CreateController();
            var users = SetupUsers();

            var result = await controller.DeleteConfirmed(1) as RedirectToActionResult;

            _userService.Verify(s => s.DeleteAsync(It.Is<User>(u => u.Id == 1)), Times.Once);
            result!.ActionName.Should().Be("List");
        }

        [Fact]
        public async Task View_ShouldReturnUserDetails_AndLogView()
        {
            var controller = CreateController();
            var users = SetupUsers();

            _userService.Setup(s => s.ViewAsync(1)).ReturnsAsync(users[0]);

            var result = await controller.View(1) as ViewResult;

            result!.ViewName.Should().Be("ViewUserDetails");
            var model = result.Model.Should().BeOfType<AddEditUserViewModel>().Which;
            model.Id.Should().Be(1);
            model.Forename.Should().Be("John");

            _userService.Verify(s => s.ViewAsync(1), Times.Once);

        }

    }
}
