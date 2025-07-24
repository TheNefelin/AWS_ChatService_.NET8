using AWS_ChatService_Application.Common;
using AWS_ChatService_Application.DTOs;
using AWS_ChatService_Application.Interfaces;
using AWS_ChatService_Application.Mappers;
using AWS_ChatService_Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AWS_ChatService_Application.Services;

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