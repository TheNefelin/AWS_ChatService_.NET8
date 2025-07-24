using AWS_ChatService_Application.Common;
using AWS_ChatService_Application.DTOs;

namespace AWS_ChatService_Application.Interfaces;

public interface IMessageService
{
    Task<ResponseApi<IEnumerable<MessageDto>>> GetMessagesAsync(Guid chatRoomId);
    Task<ResponseApi<MessageDto>> SendMessageAsync(MessageDto messageDto);
}