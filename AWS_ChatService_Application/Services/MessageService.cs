using AWS_ChatService_Application.Common;
using AWS_ChatService_Application.DTOs;
using AWS_ChatService_Application.Interfaces;
using AWS_ChatService_Application.Mappers;
using AWS_ChatService_Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AWS_ChatService_Application.Services;

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
            return ResponseApi<IEnumerable<MessageDto>>.Fail(500, $"Error al obtener mensajes: {ex.Message} | InnerException: {ex.InnerException?.Message}");
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
            return ResponseApi<MessageDto>.Fail(500, $"Error al enviar mensaje: {ex.Message}| InnerException: {ex.InnerException?.Message}");
        }
    }
}