namespace AWS_ChatService_Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? GoogleId { get; set; }
    public string? Picture { get; set; }
    public DateTime ConnectedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // Navegación
    public ICollection<Message>? Messages { get; set; }
}