namespace AWS_ChatService_Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public string GoogleId { get; set; } = default!;
    public string? Picture { get; set; }
    public string? Names { get; set; }
    public bool IsActive { get; set; }
    public DateTime ConnectedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // Navegación
    public ICollection<Message>? Messages { get; set; }
}