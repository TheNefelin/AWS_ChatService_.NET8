using AWS_ChatService_Application.DTOs;
using AWS_ChatService_Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AWS_ChatService_API.Hubs;

public class ChatHub : Hub
{
    private readonly IMessageService _messageService;

    public ChatHub(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public async Task SendMessage(MessageDto dto)
    {
        // Persistimos el mensaje
        var message = await _messageService.SendMessageAsync(dto);

        // Broadcast a todos los usuarios conectados
        await Clients.Group(dto.ChatRoomId.ToString())
        .SendAsync("ReceiveMessage", message);
    }

    public async Task JoinRoom(Guid chatRoomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomId.ToString());
    }

    public async Task LeaveRoom(Guid chatRoomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoomId.ToString());
    }
}