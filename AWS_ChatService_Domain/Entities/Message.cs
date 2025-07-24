namespace AWS_ChatService_Domain.Entities;

public class Message
{
    public Guid Id { get; set; }
    public string Content { get; set; } = default!;
    public DateTime SentAt { get; set; }

    // Relaciones
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid ChatRoomId { get; set; }
    public ChatRoom? ChatRoom { get; set; }
}