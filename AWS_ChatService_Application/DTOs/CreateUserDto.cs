namespace AWS_ChatService_Application.DTOs;

public class CreateUserDto
{
    public string Email { get; set; } = default!;
    public string GoogleId { get; set; } = default!;
}