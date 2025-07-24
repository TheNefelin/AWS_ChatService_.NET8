using AWS_ChatService_Application.Common;
using AWS_ChatService_Application.DTOs;
using AWS_ChatService_Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AWS_ChatService_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IUserService _userService;

    public UsersController(ILogger<UsersController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseApi<IEnumerable<UserDto>>>> GetAll()
    {
        _logger.LogInformation("[UsersController] - Fetching all users");
        var responseAPI = await _userService.GetAllUsersAsync();
        return StatusCode(responseAPI.StatusCode, responseAPI);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseApi<UserDto>>> GetById(Guid id)
    {
        _logger.LogInformation($"[UsersController] - Fetching user by ID: {id}");
        var responseAPI = await _userService.GetUserByIdAsync(id);
        return StatusCode(responseAPI.StatusCode, responseAPI);
    }

    [HttpPost]
    public async Task<ActionResult<ResponseApi<UserDto>>> Create(CreateUserDto createUserDto)
    {
        _logger.LogInformation("[UsersController] - Creating new user");
        var responseAPI = await _userService.CreateUserAsync(createUserDto);
        return StatusCode(responseAPI.StatusCode, responseAPI);
    }

    [HttpPut]
    public async Task<ActionResult<ResponseApi<UserDto>>> Update(UserDto userDto)
    {
        _logger.LogInformation("[UsersController] - Updating user");
        var responseAPI = await _userService.UpdateUserAsync(userDto);
        return StatusCode(responseAPI.StatusCode, responseAPI);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ResponseApi<bool>>> Delete(Guid id)
    {
        _logger.LogInformation($"[UsersController] - Deleting user with ID: {id}");
        var responseAPI = await _userService.DeleteUserAsync(id);
        return StatusCode(responseAPI.StatusCode, responseAPI);
    }
}