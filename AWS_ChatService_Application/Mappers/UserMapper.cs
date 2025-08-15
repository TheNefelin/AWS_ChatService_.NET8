using AWS_ChatService_Application.DTOs;
using AWS_ChatService_Domain.Entities;

namespace AWS_ChatService_Application.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(User user) => new UserDto
    {
        Id = user.Id,
        Email = user.Email,
        GoogleId = user.GoogleId,
        Picture = user.Picture,
        Names = user.Names,
        IsActive = user.IsActive,
        ConnectedAt = user.ConnectedAt,
        LastLoginAt = user.LastLoginAt,
    };

    public static User ToEntity(UserDto dto) => new User
    {
        Id = dto.Id,
        Email = dto.Email,
        GoogleId = dto.GoogleId,
        Picture = dto.Picture,
        Names = dto.Names,
        IsActive = dto.IsActive,
        LastLoginAt = DateTime.UtcNow
    };

    public static User ToEntity(CreateUserDto dto) => new User
    {
        Email = dto.Email,
        GoogleId = dto.GoogleId,
        LastLoginAt = DateTime.UtcNow
    };
}