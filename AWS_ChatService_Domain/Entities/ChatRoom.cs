namespace AWS_ChatService_Domain.Entities;

public class ChatRoom
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime CreatedAt { get; set; }

    // Navegación
    public ICollection<Message>? Messages { get; set; }
}