using AWS_ChatService_Application.Common;
using AWS_ChatService_Application.DTOs;
using AWS_ChatService_Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AWS_ChatService_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatRoomsController : ControllerBase
{
    private readonly ILogger<ChatRoomsController> _logger;
    private readonly IChatRoomService _chatRoomService;

    public ChatRoomsController(ILogger<ChatRoomsController> logger, IChatRoomService chatRoomService)
    {
        _logger = logger;
        _chatRoomService = chatRoomService;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseApi<IEnumerable<ChatRoomDto>>>> GetAll()
    {
        _logger.LogInformation("[ChatRoomsController] - Fetching all chat rooms");
        var responseApi = await _chatRoomService.GetAllChatRoomsAsync();
        return StatusCode(responseApi.StatusCode, responseApi);
    }

    [HttpPost]
    public async Task<ActionResult<ResponseApi<ChatRoomDto>>> Create(CreateChatRoomDto dto)
    {
        _logger.LogInformation("[ChatRoomsController] - Creating new chat room");
        var responseApi = await _chatRoomService.CreateChatRoomAsync(dto);
        return StatusCode(responseApi.StatusCode, responseApi);
    }
}