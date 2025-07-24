using AWS_ChatService_Application.Common;
using AWS_ChatService_Application.DTOs;

namespace AWS_ChatService_Application.Interfaces;

public interface IChatRoomService
{
    Task<ResponseApi<IEnumerable<ChatRoomDto>>> GetAllChatRoomsAsync();
    Task<ResponseApi<ChatRoomDto>> CreateChatRoomAsync(CreateChatRoomDto dto);
}