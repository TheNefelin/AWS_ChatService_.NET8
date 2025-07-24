namespace AWS_ChatService_Application.DTOs;

public class ChatRoomDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}