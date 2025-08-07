namespace AWS_ChatService_Application.DTOs;

public class AuthResponseDto
{
    public string Token { get; set; } = default!;
    public UserDto User { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
}
