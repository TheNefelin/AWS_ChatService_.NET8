using AWS_ChatService_Application.DTOs;
using AWS_ChatService_Application.Services;
using AWS_ChatService_Domain.Entities;
using AWS_ChatService_Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace xUnitTest.Unit.AWS_ChatService_Application;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepo;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _mockRepo = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<UserService>>();
        _service = new UserService(_mockLogger.Object, _mockRepo.Object);
    }

    [Fact]
    public async Task GetUserByIdAsync_should_return_not_found_when_user_does_not_exist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User)null!);

        // Act
        var result = await _service.GetUserByIdAsync(userId);

        // Assert
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("Usuario no encontrado", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetUserByIdAsync_should_return_user_when_exists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Email = "JohnDoe@test.com", ConnectedAt = DateTime.UtcNow };
        _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _service.GetUserByIdAsync(userId);

        // Assert
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.Equal("JohnDoe@test.com", result.Data!.Email);
    }

    [Fact]
    public async Task GetAllUsersAsync_should_return_not_found_when_no_users()
    {
        // Arrange: devuelve una lista vacía, no null
        _mockRepo.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(new List<User>());

        // Act
        var result = await _service.GetAllUsersAsync();

        // Assert
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("No se encontraron usuarios", result.Message);
        Assert.Empty(result.Data); // Esto puede ser null o vacío, depende de tu ResponseApi
    }

    [Fact]
    public async Task GetAllUsersAsync_should_return_users_when_exist()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = Guid.NewGuid(), Email = "JohnDoe@test1.com", ConnectedAt = DateTime.UtcNow },
            new User { Id = Guid.NewGuid(), Email = "JohnDoe@test2.com", ConnectedAt = DateTime.UtcNow }
        };
        _mockRepo.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(users);

        // Act
        var result = await _service.GetAllUsersAsync();

        // Assert
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data!.Count());
        Assert.Contains(result.Data, u => u.Email == "JohnDoe@test1.com");
        Assert.Contains(result.Data, u => u.Email == "JohnDoe@test2.com");
    }

    [Fact]
    public async Task CreateUserAsync_should_create_and_return_user()
    {
        // Arrange
        var createUserDto = new CreateUserDto { Email = "JohnDoe@test.com" };
        User? savedUser = null;
        _mockRepo.Setup(r => r.CreateUserAsync(It.IsAny<User>()))
                 .Callback<User>(u => savedUser = u)
                 .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateUserAsync(createUserDto);

        // Assert
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.Equal("JohnDoe@test.com", result.Data!.Email);
        Assert.Equal(savedUser!.Id, result.Data.Id);
    }

    [Fact]
    public async Task UpdateUserAsync_should_update_and_return_user()
    {
        // Arrange
        var userDto = new UserDto { Id = Guid.NewGuid(), Email = "JohnDoe@test.com", ConnectedAt = DateTime.UtcNow };
        User? updatedUser = null;
        _mockRepo.Setup(r => r.UpdateUserAsync(It.IsAny<User>()))
                 .Callback<User>(u => updatedUser = u)
                 .Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateUserAsync(userDto);

        // Assert
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.Equal("JohnDoe@test.com", result.Data!.Email);
        Assert.Equal(updatedUser!.Id, result.Data.Id);
    }

    [Fact]
    public async Task DeleteUserAsync_should_fail_when_user_has_messages()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockRepo.Setup(r => r.HasMessagesAsync(userId)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteUserAsync(userId);

        // Assert
        Assert.Equal(400, result.StatusCode);
        Assert.False(result.Data);
        Assert.Equal("No se puede eliminar el usuario porque tiene mensajes asociados.", result.Message);
    }

    [Fact]
    public async Task DeleteUserAsync_should_delete_and_return_true()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockRepo.Setup(r => r.HasMessagesAsync(userId)).ReturnsAsync(false);
        _mockRepo.Setup(r => r.DeleteUserAsync(userId)).Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteUserAsync(userId);

        // Assert
        Assert.Equal(204, result.StatusCode);
        Assert.True(result.Data);
    }
}