using System.Threading.Tasks;
using UserManagement.Models;

namespace UserManagement.Data.Tests;

public class DataContextTests
{
    [Fact]
    public async Task GetAll_WhenNewEntityAdded_ShouldIncludeNewEntity()
    {
        // Arrange
        var context = CreateContext();
        var entity = new User
        {
            Forename = "Brand New",
            Surname = "User",
            Email = "brandnewuser@example.com"
        };

        // Act
        await context.CreateAsync(entity);
        var result = context.GetAll<User>();

        // Assert
        result.Should().ContainSingle(u => u.Email == entity.Email)
              .Which.Should().BeEquivalentTo(entity);
    }

    [Fact]
    public async Task GetAll_WhenEntityDeleted_ShouldNotIncludeDeletedEntity()
    {
        // Arrange
        var context = CreateContext();
        var existingUser = new User
        {
            Forename = "Temp",
            Surname = "User",
            Email = "tempuser@example.com"
        };
        await context.CreateAsync(existingUser);

        // Act
        await context.DeleteAsync(existingUser);
        var result = context.GetAll<User>();

        // Assert
        result.Should().NotContain(u => u.Email == existingUser.Email);
    }

    private DataContext CreateContext() => new();
}
