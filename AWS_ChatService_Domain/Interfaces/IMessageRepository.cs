using AWS_ChatService_Domain.Entities;

namespace AWS_ChatService_Domain.Interfaces;

public interface IMessageRepository
{
    Task<IEnumerable<Message>> GetMessagesByChatRoomAsync(Guid chatRoomId);
    Task SendMessageAsync(Message message);
}