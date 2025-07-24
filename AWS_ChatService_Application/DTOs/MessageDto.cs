namespace AWS_ChatService_Application.DTOs;

public class MessageDto
{
    public Guid ChatRoomId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = default!;
}