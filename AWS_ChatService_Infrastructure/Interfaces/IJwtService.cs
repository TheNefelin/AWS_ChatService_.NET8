using AWS_ChatService_Domain.DTOs;
using AWS_ChatService_Domain.Entities;
using System.Security.Claims;

namespace AWS_ChatService_Infrastructure.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user, JwtConfigData jwtConfigData);
    ClaimsPrincipal? ValidateToken(string token);
}