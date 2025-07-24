using AWS_ChatService_Application.DTOs;
using AWS_ChatService_Application.Services;
using AWS_ChatService_Domain.Entities;
using AWS_ChatService_Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace xUnitTest.Unit.AWS_ChatService_Application;

public class MessageServiceTests
{
    private readonly Mock<IMessageRepository> _mockRepo;
    private readonly Mock<ILogger<MessageService>> _mockLogger;
    private readonly MessageService _service;

    public MessageServiceTests()
    {
        _mockRepo = new Mock<IMessageRepository>();
        _mockLogger = new Mock<ILogger<MessageService>>();
        _service = new MessageService(_mockLogger.Object, _mockRepo.Object);
    }

    [Fact]
    public async Task GetMessagesAsync_should_return_not_found_when_no_messages()
    {
        // Arrange
        var chatRoomId = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetMessagesByChatRoomAsync(chatRoomId)).ReturnsAsync(new List<Message>());

        // Act
        var result = await _service.GetMessagesAsync(chatRoomId);

        // Assert
        Assert.Equal(404, result.StatusCode);
        Assert.Empty(result.Data);
        Assert.Equal("No se encontraron mensajes", result.Message);
    }

    [Fact]
    public async Task GetMessagesAsync_should_return_messages_when_exist()
    {
        // Arrange
        var chatRoomId = Guid.NewGuid();
        var messages = new List<Message>
        {
            new Message { Id = Guid.NewGuid(), Content = "Hola", ChatRoomId = chatRoomId, UserId = Guid.NewGuid(), SentAt = DateTime.UtcNow },
            new Message { Id = Guid.NewGuid(), Content = "¿Qué tal?", ChatRoomId = chatRoomId, UserId = Guid.NewGuid(), SentAt = DateTime.UtcNow }
        };
        _mockRepo.Setup(r => r.GetMessagesByChatRoomAsync(chatRoomId)).ReturnsAsync(messages);

        // Act
        var result = await _service.GetMessagesAsync(chatRoomId);

        // Assert
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data!.Count());
    }

    [Fact]
    public async Task SendMessageAsync_should_send_and_return_message()
    {
        // Arrange
        var messageDto = new MessageDto
        {
            Content = "Test",
            ChatRoomId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
        };
        Message? sentMessage = null;
        _mockRepo.Setup(r => r.SendMessageAsync(It.IsAny<Message>()))
                 .Callback<Message>(m => sentMessage = m)
                 .Returns(Task.CompletedTask);

        // Act
        var result = await _service.SendMessageAsync(messageDto);

        // Assert
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.Equal(messageDto.Content, result.Data!.Content);
        Assert.Equal(sentMessage!.Content, result.Data.Content);
    }
}