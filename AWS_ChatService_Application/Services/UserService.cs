using AWS_ChatService_Application.Common;
using AWS_ChatService_Application.DTOs;
using AWS_ChatService_Application.Interfaces;
using AWS_ChatService_Application.Mappers;
using AWS_ChatService_Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AWS_ChatService_Application.Services;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IUserRepository _userRepository;

    public UserService(ILogger<UserService> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task<ResponseApi<IEnumerable<UserDto>>> GetAllUsersAsync()
    {
        try
        {
            _logger.LogInformation("[UserService] - Obteniendo todos los usuarios");
            var users = await _userRepository.GetAllUsersAsync();

            if (!users.Any())
            {
                _logger.LogWarning("[UserService] - No se encontraron usuarios");
                return ResponseApi<IEnumerable<UserDto>>.Fail(404, "No se encontraron usuarios", Enumerable.Empty<UserDto>());
            }

            _logger.LogInformation($"[UserService] - Se encontraron {users.Count()} usuarios");
            var userDtos = users.Select(UserMapper.ToDto);
            return ResponseApi<IEnumerable<UserDto>>.Success(userDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UserService] - Error al obtener usuarios");
            return ResponseApi<IEnumerable<UserDto>>.Fail(500, $"[UserService] - Error interno: {ex.Message} | InnerException: {ex.InnerException?.Message}");
        }
    }

    public async Task<ResponseApi<UserDto?>> GetUserByIdAsync(Guid userId)
    {
        try
        {
            _logger.LogInformation($"[UserService] - Obteniendo usuario con ID: {userId}");
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning($"[UserService] - Usuario con ID {userId} no encontrado");
                return ResponseApi<UserDto?>.Fail(404, "Usuario no encontrado");
            }

            _logger.LogInformation($"[UserService] - Usuario encontrado: {user.Email}");
            return ResponseApi<UserDto?>.Success(UserMapper.ToDto(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[UserService] - Error al obtener usuario con ID {userId}");
            return ResponseApi<UserDto?>.Fail(500, $"Error interno: {ex.Message} | InnerException: {ex.InnerException?.Message}");
        }
    }

    public async Task<ResponseApi<UserDto>> CreateUserAsync(CreateUserDto createUserDto)
    {
        try
        {
            _logger.LogInformation($"[UserService] - Creando usuario: {createUserDto.Email}");
            if (string.IsNullOrWhiteSpace(createUserDto.Email))
            {
                _logger.LogWarning("[UserService] - Username es requerido para crear un usuario");
                return ResponseApi<UserDto>.Fail(400, "Username es requerido");
            }

            var user = UserMapper.ToEntity(createUserDto);
            await _userRepository.CreateUserAsync(user);
            return ResponseApi<UserDto>.Success(UserMapper.ToDto(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[UserService] - Error al crear usuario: {createUserDto.Email}");
            return ResponseApi<UserDto>.Fail(500, $"Error interno: {ex.Message} | InnerException: {ex.InnerException?.Message}");
        }
    }

    public async Task<ResponseApi<UserDto>> UpdateUserAsync(UserDto userDto)
    {
        try
        {
            _logger.LogInformation($"[UserService] - Actualizando usuario: {userDto.Email}");
            var user = UserMapper.ToEntity(userDto);
            await _userRepository.UpdateUserAsync(user);
            return ResponseApi<UserDto>.Success(UserMapper.ToDto(user), 201);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[UserService] - Error al actualizar usuario: {userDto.Email}");
            return ResponseApi<UserDto>.Fail(500, $"Error interno: {ex.Message} | InnerException: {ex.InnerException?.Message}");
        }
    }

    public async Task<ResponseApi<bool>> DeleteUserAsync(Guid userId)
    {
        try
        {
            _logger.LogInformation($"[UserService] - Validando si el usuario con ID {userId} puede ser eliminado");
            if (await _userRepository.HasMessagesAsync(userId))
            {
                _logger.LogWarning($"[UserService] - No se puede eliminar el usuario con ID {userId} porque tiene mensajes asociados.");
                return ResponseApi<bool>.Fail(400, "No se puede eliminar el usuario porque tiene mensajes asociados.");
            }

            _logger.LogInformation($"[UserService] - Eliminando usuario con ID: {userId}");
            await _userRepository.DeleteUserAsync(userId);
            return ResponseApi<bool>.Success(true, 204);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[UserService] - Error al eliminar usuario con ID {userId}");
            return ResponseApi<bool>.Fail(500, $"Error interno: {ex.Message} | InnerException: {ex.InnerException?.Message}");
        }
    }
}