using AWS_ChatService_Application.Common;
using AWS_ChatService_Application.DTOs;

namespace AWS_ChatService_Application.Interfaces;

public interface IUserService
{
    Task<ResponseApi<IEnumerable<UserDto>>> GetAllUsersAsync();
    Task<ResponseApi<UserDto>> GetUserByIdAsync(Guid userId);
    Task<ResponseApi<UserDto>> CreateUserAsync(CreateUserDto createUserDto);
    Task<ResponseApi<UserDto>> UpdateUserAsync(UserDto userDto);
    Task<ResponseApi<bool>> DeleteUserAsync(Guid userId);
}