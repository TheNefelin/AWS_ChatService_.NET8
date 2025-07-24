using AWS_ChatService_Application.DTOs;
using AWS_ChatService_Domain.Entities;

namespace AWS_ChatService_Application.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(User user) => new UserDto
    {
        Id = user.Id,
        Username = user.Username,
        ConnectedAt = user.ConnectedAt
    };

    public static User ToEntity(UserDto dto) => new User
    {
        Id = dto.Id,
        Username = dto.Username,
        ConnectedAt = dto.ConnectedAt
    };

    public static User ToEntity(CreateUserDto dto) => new User
    {
        Id = Guid.NewGuid(),
        Username = dto.Username,
        ConnectedAt = DateTime.UtcNow
    };
}