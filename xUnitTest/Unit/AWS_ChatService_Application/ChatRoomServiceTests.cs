using AWS_ChatService_Application.DTOs;
using AWS_ChatService_Application.Services;
using AWS_ChatService_Domain.Entities;
using AWS_ChatService_Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace xUnitTest.Unit.AWS_ChatService_Application;

public class ChatRoomServiceTests
{
    private readonly Mock<IChatRoomRepository> _mockRepo;
    private readonly Mock<ILogger<ChatRoomService>> _mockLogger;
    private readonly ChatRoomService _service;

    public ChatRoomServiceTests()
    {
        _mockRepo = new Mock<IChatRoomRepository>();
        _mockLogger = new Mock<ILogger<ChatRoomService>>();
        _service = new ChatRoomService(_mockLogger.Object, _mockRepo.Object);
    }

    [Fact]
    public async Task GetAllChatRoomsAsync_should_return_not_found_when_empty()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetAllChatRoomsAsync()).ReturnsAsync(new List<ChatRoom>());

        // Act
        var result = await _service.GetAllChatRoomsAsync();

        // Assert
        Assert.Equal(404, result.StatusCode);
        Assert.Empty(result.Data);
        Assert.Equal("No chat rooms found", result.Message);
    }

    [Fact]
    public async Task GetAllChatRoomsAsync_should_return_chat_rooms_when_exist()
    {
        // Arrange
        var rooms = new List<ChatRoom>
        {
            new ChatRoom { Id = Guid.NewGuid(), Name = "Sala 1", CreatedAt = DateTime.UtcNow },
            new ChatRoom { Id = Guid.NewGuid(), Name = "Sala 2", CreatedAt = DateTime.UtcNow }
        };
        _mockRepo.Setup(r => r.GetAllChatRoomsAsync()).ReturnsAsync(rooms);

        // Act
        var result = await _service.GetAllChatRoomsAsync();

        // Assert
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(2, result.Data!.Count());
    }

    [Fact]
    public async Task CreateChatRoomAsync_should_create_and_return_chat_room()
    {
        // Arrange
        var dto = new CreateChatRoomDto { Name = "Nueva Sala" };
        ChatRoom? createdRoom = null;
        _mockRepo.Setup(r => r.CreateChatRoomAsync(It.IsAny<ChatRoom>()))
                 .Callback<ChatRoom>(cr => createdRoom = cr)
                 .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateChatRoomAsync(dto);

        // Assert
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.Equal("Nueva Sala", result.Data!.Name);
        Assert.Equal(createdRoom!.Name, result.Data.Name);
    }
}