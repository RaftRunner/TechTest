using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;
using UserManagement.Services.Interfaces;

namespace UserManagement.Data.Tests;

public class UserServiceTests
{
    private readonly Mock<IDataContext> _dataContext = new();
    private readonly Mock<ILogService> _logService = new();

    private UserService CreateService() => new(_dataContext.Object, _logService.Object);


    [Fact]
    public async Task GetAllAsync_WhenContextReturnsUsers_ShouldReturnSameUsers()
    {
        // Arrange
        var service = CreateService();
        var users = SetupUsers();

        // Act
        var result = await service.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(users);
    }

    private IQueryable<User> SetupUsers(string forename = "Johnny", string surname = "User", string email = "juser@example.com", bool isActive = true)
    {
        var users = new[]
        {
            new User
            {
                Forename = forename,
                Surname = surname,
                Email = email,
                IsActive = isActive
            }
        }.AsQueryable();

        _dataContext.Setup(s => s.GetAll<User>()).Returns(users);

        return users;
    }
}
