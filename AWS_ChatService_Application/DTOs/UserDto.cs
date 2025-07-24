namespace AWS_ChatService_Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public DateTime ConnectedAt { get; set; }
}