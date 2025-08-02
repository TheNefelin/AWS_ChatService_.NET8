## AWS_ChatService_Domain

- Entities

```csharp
public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public DateTime ConnectedAt { get; set; }

    // Navegación
    public ICollection<Message>? Messages { get; set; }
}
```
```csharp
public class Message
{
    public Guid Id { get; set; }
    public string Content { get; set; } = default!;
    public DateTime SentAt { get; set; }

    // Relaciones
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid ChatRoomId { get; set; }
    public ChatRoom? ChatRoom { get; set; }
}
```
```csharp
public class ChatRoom
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime CreatedAt { get; set; }

    // Navegación
    public ICollection<Message>? Messages { get; set; }
}
```

- Interfaces

```csharp
public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User> GetUserByIdAsync(Guid userId);
    Task CreateUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(Guid userId);
    Task<bool> HasMessagesAsync(Guid userId);
}
```
```csharp
public interface IMessageRepository
{
    Task<IEnumerable<Message>> GetMessagesByChatRoomAsync(Guid chatRoomId);
    Task SendMessageAsync(Message message);
}
```
```csharp
public interface IChatRoomRepository
{
    Task<IEnumerable<ChatRoom>> GetAllChatRoomsAsync();
    Task<ChatRoom?> GetChatRoomByIdAsync(Guid chatRoomId);
    Task CreateChatRoomAsync(ChatRoom chatRoom);
}
```

## AWS_ChatService_Infrastructure

- Repositories

```csharp
public class UserRepository : IUserRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly DapperConnectionFactory _dapperConnectionFactory;

    public UserRepository(ILogger<UserRepository> logger, DapperConnectionFactory dapperConnectionFactory)
    {
        _logger = logger;
        _dapperConnectionFactory = dapperConnectionFactory;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        try
        {
            _logger.LogInformation("[UserRepository] - Fetching all users from the database");
            const string sql = "SELECT * FROM Users ORDER BY ConnectedAt DESC";
            using var conn = _dapperConnectionFactory.CreateConnection();
            return await conn.QueryAsync<User>(sql);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UserRepository] - Error fetching all users");
            throw; // re-lanza para que lo maneje la capa Application
        }
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        try
        {
            _logger.LogInformation($"[UserRepository] - Fetching user by ID: {userId}");
            const string sql = "SELECT * FROM Users WHERE Id = @userId";
            using var conn = _dapperConnectionFactory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<User>(sql, new { userId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UserRepository] - Error fetching user by ID");
            throw;
        }
    }


    public async Task CreateUserAsync(User user)
    {
        try
        {
            _logger.LogInformation($"[UserRepository] - Creating user: {user.Username}");
            const string sql = @"INSERT INTO Users (Id, Username, ConnectedAt) VALUES (@Id, @Username, @ConnectedAt)";
            using var conn = _dapperConnectionFactory.CreateConnection();
            await conn.ExecuteAsync(sql, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UserRepository] - Error creating user");
            throw;
        }
    }

    public async Task UpdateUserAsync(User user)
    {
        try
        {
            _logger.LogInformation($"[UserRepository] - Updating user: {user.Username}");
            const string sql = @"UPDATE Users SET Username = @Username, ConnectedAt = @ConnectedAt WHERE Id = @Id";
            using var conn = _dapperConnectionFactory.CreateConnection();
            await conn.ExecuteAsync(sql, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UserRepository] - Error updating user");
            throw;
        }
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        try
        {
            _logger.LogInformation($"[UserRepository] - Deleting user with ID: {userId}");
            const string query = @"DELETE FROM Users WHERE Id = @userId";
            using var conn = _dapperConnectionFactory.CreateConnection();
            await conn.ExecuteAsync(query, new { userId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UserRepository] - Error deleting user");
            throw;
        }
    }

    public async Task<bool> HasMessagesAsync(Guid userId)
    {
        try
        {
            _logger.LogInformation($"[UserRepository] - Checking if user with ID {userId} has messages");
            const string sql = @"SELECT 1 FROM Messages WHERE UserId = @userId LIMIT 1";
            using var conn = _dapperConnectionFactory.CreateConnection();
            return await conn.ExecuteScalarAsync<int?>(sql, new { userId }) != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UserRepository] - Error checking if user has messages");
            throw;
        }
    }
}
```
```csharp
public class MessageRepository : IMessageRepository
{
    private readonly ILogger<MessageRepository> _logger;
    private readonly DapperConnectionFactory _connectionFactory;

    public MessageRepository(ILogger<MessageRepository> logger, DapperConnectionFactory connectionFactory)
    {
        _logger = logger;
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Message>> GetMessagesByChatRoomAsync(Guid chatRoomId)
    {
        try
        {
            _logger.LogInformation("[MessageRepository] - Fetching messages for chat room {ChatRoomId}", chatRoomId);

            const string query = @"
            SELECT * FROM Messages 
            WHERE ChatRoomId = @ChatRoomId 
            ORDER BY SentAt ASC";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Message>(query, new { ChatRoomId = chatRoomId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[MessageRepository] - Error fetching messages for chat room {ChatRoomId}", chatRoomId);
            throw; // Re-throw to be handled by the Application layer
        }
    }

    public async Task SendMessageAsync(Message message)
    {
        try
        {
            _logger.LogInformation("[MessageRepository] - Creating message with ID {MessageId} for chat room {ChatRoomId}", message.Id, message.ChatRoomId);

            const string sql = @"
            INSERT INTO Messages (Id, Content, SentAt, UserId, ChatRoomId)
            VALUES (@Id, @Content, @SentAt, @UserId, @ChatRoomId)";

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[MessageRepository] - Error creating message with ID {MessageId}", message.Id);
            throw;
        }
    }
}
```
```csharp
public class ChatRoomRepository : IChatRoomRepository
{
    private readonly ILogger<ChatRoomRepository> _logger;
    private readonly DapperConnectionFactory _dapperConnectionFactory;

    public ChatRoomRepository(ILogger<ChatRoomRepository> logger, DapperConnectionFactory dapperConnectionFactory)
    {
        _logger = logger;
        _dapperConnectionFactory = dapperConnectionFactory;
    }

    public async Task<IEnumerable<ChatRoom>> GetAllChatRoomsAsync()
    {
        try
        {
            _logger.LogInformation("[ChatRoomRepository] - Fetching all chat rooms");

            const string sql = "SELECT * FROM ChatRooms ORDER BY CreatedAt DESC";
            using var connection = _dapperConnectionFactory.CreateConnection();
            return await connection.QueryAsync<ChatRoom>(sql);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ChatRoomRepository] - Error fetching all chat rooms");
            throw; // Re-throw to be handled by the Application layer
        }
    }

    public async Task<ChatRoom?> GetChatRoomByIdAsync(Guid chatRoomId)
    {
        try
        {
            _logger.LogInformation("[ChatRoomRepository] - Fetching chat room with ID {ChatRoomId}", chatRoomId);

            const string sql = "SELECT * FROM ChatRooms WHERE Id = @ChatRoomId";
            using var connection = _dapperConnectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<ChatRoom>(sql, new { ChatRoomId = chatRoomId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ChatRoomRepository] - Error fetching chat room with ID {ChatRoomId}", chatRoomId);
            throw;
        }
    }

    public async Task CreateChatRoomAsync(ChatRoom room)
    {
        try
        {
            _logger.LogInformation("[ChatRoomRepository] - Creating chat room with ID {ChatRoomId}", room.Id);

            const string sql = @"INSERT INTO ChatRooms (Id, Name, CreatedAt) VALUES (@Id, @Name, @CreatedAt)";
            using var conn = _dapperConnectionFactory.CreateConnection();
            await conn.ExecuteAsync(sql, room);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ChatRoomRepository] - Error creating chat room with ID {ChatRoomId}", room.Id);
            throw;
        }
    }
}
```

- Configuration

```csharp
public class DapperConnectionFactory
{
    private readonly string _connectionString;

    public DapperConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("PostgreSQL")
            ?? throw new ArgumentNullException("Connection string not found");
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
```

## AWS_ChatService_Application

- Common

```csharp
public class ResponseApi<T>
{
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public static ResponseApi<T> Success(T data, int statusCode = 200, string message = "Ok")
    {
        return new ResponseApi<T>
        {
            IsSuccess = true,
            StatusCode = statusCode,
            Message = message,
            Data = data
        };
    }

    public static ResponseApi<T> Fail(int statusCode, string message, T data = default)
    {
        return new ResponseApi<T>
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Message = message,
            Data = data
        };
    }
}
```

- DTOs

```csharp
public class ChatRoomDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}
```
```csharp
public class CreateChatRoomDto
{
    public string Name { get; set; } = default!;
}
```
```csharp
public class CreateUserDto
{
    public string Username { get; set; } = default!;
}
```
```csharp
public class MessageDto
{
    public Guid ChatRoomId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = default!;
}
```
```csharp
public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public DateTime ConnectedAt { get; set; }
}
```

- Interfaces

```csharp
public interface IUserService
{
    Task<ResponseApi<IEnumerable<UserDto>>> GetAllUsersAsync();
    Task<ResponseApi<UserDto>> GetUserByIdAsync(Guid userId);
    Task<ResponseApi<UserDto>> CreateUserAsync(CreateUserDto createUserDto);
    Task<ResponseApi<UserDto>> UpdateUserAsync(UserDto userDto);
    Task<ResponseApi<bool>> DeleteUserAsync(Guid userId);
}
```
```csharp
public interface IMessageService
{
    Task<ResponseApi<IEnumerable<MessageDto>>> GetMessagesAsync(Guid chatRoomId);
    Task<ResponseApi<MessageDto>> SendMessageAsync(MessageDto messageDto);
}
```
```csharp
public interface IChatRoomService
{
    Task<ResponseApi<IEnumerable<ChatRoomDto>>> GetAllChatRoomsAsync();
    Task<ResponseApi<ChatRoomDto>> CreateChatRoomAsync(CreateChatRoomDto dto);
}
```

- Mappers

```csharp
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
```
```csharp
public class MessageMapper
{
    public static MessageDto ToDto(Message message) => new MessageDto
    {
        ChatRoomId = message.ChatRoomId,
        UserId = message.UserId,
        Content = message.Content,
    };

    public static Message ToEntity(MessageDto messageDto) => new Message
    {
        Id = Guid.NewGuid(),
        ChatRoomId = messageDto.ChatRoomId,
        UserId = messageDto.UserId,
        Content = messageDto.Content,
        SentAt = DateTime.UtcNow
    };
}
```
```csharp
public class ChatRoomMapper
{
    public static ChatRoomDto ToDto(ChatRoom chatRoom) => new ChatRoomDto
    {
        Id = chatRoom.Id,
        Name = chatRoom.Name,
        CreatedAt = chatRoom.CreatedAt
    };
    public static ChatRoom ToEntity(ChatRoomDto dto) => new ChatRoom
    {
        Id = dto.Id,
        Name = dto.Name,
        CreatedAt = dto.CreatedAt
    };
    public static ChatRoom ToEntity(CreateChatRoomDto dto) => new ChatRoom
    {
        Id = Guid.NewGuid(),
        Name = dto.Name,
        CreatedAt = DateTime.UtcNow
    };
}
```

- Services

```csharp
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
            return ResponseApi<IEnumerable<UserDto>>.Fail(500, $"[UserService] - Error interno: {ex.Message}");
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

            _logger.LogInformation($"[UserService] - Usuario encontrado: {user.Username}");
            return ResponseApi<UserDto?>.Success(UserMapper.ToDto(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[UserService] - Error al obtener usuario con ID {userId}");
            return ResponseApi<UserDto?>.Fail(500, $"Error interno: {ex.Message}");
        }
    }

    public async Task<ResponseApi<UserDto>> CreateUserAsync(CreateUserDto createUserDto)
    {
        try
        {
            _logger.LogInformation($"[UserService] - Creando usuario: {createUserDto.Username}");
            if (string.IsNullOrWhiteSpace(createUserDto.Username))
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
            _logger.LogError(ex, $"[UserService] - Error al crear usuario: {createUserDto.Username}");
            return ResponseApi<UserDto>.Fail(500, $"Error interno: {ex.Message}");
        }
    }

    public async Task<ResponseApi<UserDto>> UpdateUserAsync(UserDto userDto)
    {
        try
        {
            _logger.LogInformation($"[UserService] - Actualizando usuario: {userDto.Username}");
            var user = UserMapper.ToEntity(userDto);
            await _userRepository.UpdateUserAsync(user);
            return ResponseApi<UserDto>.Success(UserMapper.ToDto(user), 201);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[UserService] - Error al actualizar usuario: {userDto.Username}");
            return ResponseApi<UserDto>.Fail(500, $"Error interno: {ex.Message}");
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
            return ResponseApi<bool>.Fail(500, $"Error interno: {ex.Message}");
        }
    }
}
```
```csharp
public class MessageService : IMessageService
{
    private readonly ILogger<MessageService> _logger;
    private readonly IMessageRepository _messageRepository;

    public MessageService(ILogger<MessageService> logger, IMessageRepository messageRepository)
    {
        _logger = logger;
        _messageRepository = messageRepository;
    }

    public async Task<ResponseApi<IEnumerable<MessageDto>>> GetMessagesAsync(Guid chatRoomId)
    {
        try
        {
            _logger.LogInformation($"[MessageService] - Obteniendo mensajes del chat room {chatRoomId}");
            var messages = await _messageRepository.GetMessagesByChatRoomAsync(chatRoomId);

            if (!messages.Any())
            {
                _logger.LogWarning($"[MessageService] - No se encontraron mensajes para el chat room {chatRoomId}");
                return ResponseApi<IEnumerable<MessageDto>>.Fail(404, "No se encontraron mensajes", Enumerable.Empty<MessageDto>());
            }

            var messageDtos = messages.Select(MessageMapper.ToDto);
            return ResponseApi<IEnumerable<MessageDto>>.Success(messageDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[MessageService] - Error al obtener mensajes del chat room {chatRoomId}");
            return ResponseApi<IEnumerable<MessageDto>>.Fail(500, $"Error al obtener mensajes: {ex.Message}");
        }
    }

    public async Task<ResponseApi<MessageDto>> SendMessageAsync(MessageDto messageDto)
    {
        try
        {
            _logger.LogInformation($"[MessageService] - Enviando mensaje al chat room {messageDto.ChatRoomId} por el usuario {messageDto.UserId}");
            var message = MessageMapper.ToEntity(messageDto);
            await _messageRepository.SendMessageAsync(message);
            return ResponseApi<MessageDto>.Success(MessageMapper.ToDto(message), 201);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[MessageService] - Error al enviar mensaje al chat room {messageDto.ChatRoomId}");
            return ResponseApi<MessageDto>.Fail(500, $"Error al enviar mensaje: {ex.Message}");
        }
    }
}
```
```csharp
public class ChatRoomService : IChatRoomService
{
    private readonly ILogger<ChatRoomService> _logger;
    private readonly IChatRoomRepository _chatRoomRepository;

    public ChatRoomService(ILogger<ChatRoomService> logger, IChatRoomRepository chatRoomRepository)
    {
        _logger = logger;
        _chatRoomRepository = chatRoomRepository;
    }

    public async Task<ResponseApi<IEnumerable<ChatRoomDto>>> GetAllChatRoomsAsync()
    {
        try
        {
            var chatRooms = await _chatRoomRepository.GetAllChatRoomsAsync();

            if (!chatRooms.Any())
            {
                _logger.LogWarning("[ChatRoomService] - No chat rooms found");
                return ResponseApi<IEnumerable<ChatRoomDto>>.Fail(404, "No chat rooms found", Enumerable.Empty<ChatRoomDto>());
            }

            _logger.LogInformation($"[ChatRoomService] - Found {chatRooms.Count()} chat rooms");
            var chatRoomDtos = chatRooms.Select(ChatRoomMapper.ToDto);
            return ResponseApi<IEnumerable<ChatRoomDto>>.Success(chatRoomDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ChatRoomService] - Error fetching chat rooms");
            return ResponseApi<IEnumerable<ChatRoomDto>>.Fail(500, $"Internal server error: {ex.Message}");
        }
    }

    public async Task<ResponseApi<ChatRoomDto>> CreateChatRoomAsync(CreateChatRoomDto createChatRoomDto)
    {
        try
        {
            _logger.LogInformation($"[ChatRoomService] - Creating chat room with name: {createChatRoomDto.Name}");
            var chatRoom = ChatRoomMapper.ToEntity(createChatRoomDto);
            await _chatRoomRepository.CreateChatRoomAsync(chatRoom);
            return ResponseApi<ChatRoomDto>.Success(ChatRoomMapper.ToDto(chatRoom), 201);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[ChatRoomService] - Error creating chat room: {createChatRoomDto.Name}");
            return ResponseApi<ChatRoomDto>.Fail(500, $"Internal server error: {ex.Message}");
        }
    }
}
```

## AWS_ChatService_API

- Controllers

```csharp
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
```
```csharp
[Route("api/[controller]")]
[ApiController]
public class MessagesController : ControllerBase
{
    private readonly ILogger<MessagesController> _logger;
    private readonly IMessageService _messageService;

    public MessagesController(ILogger<MessagesController> logger, IMessageService messageService)
    {
        _logger = logger;
        _messageService = messageService;
    }

    [HttpGet("{chatRoomId:guid}")]
    public async Task<ActionResult<ResponseApi<IEnumerable<MessageDto>>>> GetMessages(Guid chatRoomId)
    {
        _logger.LogInformation($"[MessagesController] - Fetching messages for chat room {chatRoomId}");
        var responseApi = await _messageService.GetMessagesAsync(chatRoomId);
        return StatusCode(responseApi.StatusCode, responseApi);
    }

    [HttpPost]
    public async Task<ActionResult<ResponseApi<MessageDto>>> SendMessage(MessageDto messageDto)
    {
        _logger.LogInformation($"[MessagesController] - Sending message to chat room {messageDto.ChatRoomId} by user {messageDto.UserId}");
        var responseApi = await _messageService.SendMessageAsync(messageDto);
        return StatusCode(responseApi.StatusCode, responseApi);
    }
}
```
```csharp
[Route("api/[controller]")]
[ApiController]
public class ChatRoomsController : ControllerBase
{
    private readonly ILogger<ChatRoomsController> _logger;
    private readonly IChatRoomService _chatRoomService;

    public ChatRoomsController(ILogger<ChatRoomsController> logger, IChatRoomService chatRoomService)
    {
        _logger = logger;
        _chatRoomService = chatRoomService;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseApi<IEnumerable<ChatRoomDto>>>> GetAll()
    {
        _logger.LogInformation("[ChatRoomsController] - Fetching all chat rooms");
        var responseApi = await _chatRoomService.GetAllChatRoomsAsync();
        return StatusCode(responseApi.StatusCode, responseApi);
    }

    [HttpPost]
    public async Task<ActionResult<ResponseApi<ChatRoomDto>>> Create(CreateChatRoomDto dto)
    {
        _logger.LogInformation("[ChatRoomsController] - Creating new chat room");
        var responseApi = await _chatRoomService.CreateChatRoomAsync(dto);
        return StatusCode(responseApi.StatusCode, responseApi);
    }
}
```

- Hubs

```csharp
public class ChatHub : Hub
{
    private readonly IMessageService _messageService;

    public ChatHub(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public async Task SendMessage(MessageDto dto)
    {
        // Persistimos el mensaje
        var message = await _messageService.SendMessageAsync(dto);

        // Broadcast a todos los usuarios conectados
        await Clients.Group(dto.ChatRoomId.ToString())
        .SendAsync("ReceiveMessage", message);
    }

    public async Task JoinRoom(Guid chatRoomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomId.ToString());
    }

    public async Task LeaveRoom(Guid chatRoomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoomId.ToString());
    }
}
```

- Program.cs
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. Agregar configuración para PostgreSQL (desde appsettings.json)
builder.Services.AddSingleton<DapperConnectionFactory>();

// 2. Repositorios e interfaces
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IChatRoomRepository, ChatRoomRepository>();

// 3. Servicios de aplicación
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IChatRoomService, ChatRoomService>();

// 4. Servicios de SignalR
builder.Services.AddSignalR();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 5 CORS
var allowedOrigins = "_allowedOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowedOrigins,
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:3000",
                    "http://127.0.0.1:3000",
                    "http://localhost:4200",    // Añade tu puerto de Angular
                    "http://127.0.0.1:4200"     // Añade tu puerto de Angular
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .SetIsOriginAllowed(_ => true);
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 6 Enable CORS
app.UseCors(allowedOrigins);

app.UseAuthorization();

// 7. ChatHub Endpint
app.MapHub<ChatHub>("/chatHub");

app.MapControllers();

app.Run();
```
