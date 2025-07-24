using AWS_ChatService_Application.Common;
using AWS_ChatService_Application.DTOs;
using AWS_ChatService_Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AWS_ChatService_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MessagesController : ControllerBase
{
    private readonly ILogger<MessagesController> _logger;
    private readonly IMessageService _messageService;

    public MessagesController(ILogger<MessagesController> logger, IMessageService messageService)
    {
        _logger = logger;
        _messageService = messageService;
    }

    [HttpGet("{chatRoomId:guid}")]
    public async Task<ActionResult<ResponseApi<IEnumerable<MessageDto>>>> GetMessages(Guid chatRoomId)
    {
        _logger.LogInformation($"[MessagesController] - Fetching messages for chat room {chatRoomId}");
        var responseApi = await _messageService.GetMessagesAsync(chatRoomId);
        return StatusCode(responseApi.StatusCode, responseApi);
    }

    [HttpPost]
    public async Task<ActionResult<ResponseApi<MessageDto>>> SendMessage(MessageDto messageDto)
    {
        _logger.LogInformation($"[MessagesController] - Sending message to chat room {messageDto.ChatRoomId} by user {messageDto.UserId}");
        var responseApi = await _messageService.SendMessageAsync(messageDto);
        return StatusCode(responseApi.StatusCode, responseApi);
    }
}