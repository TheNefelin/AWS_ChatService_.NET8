namespace AWS_ChatService_Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public DateTime ConnectedAt { get; set; }

    // Navegación
    public ICollection<Message>? Messages { get; set; }
}