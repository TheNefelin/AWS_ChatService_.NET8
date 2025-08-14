using AWS_ChatService_Domain.DTOs;
using AWS_ChatService_Domain.Entities;
using AWS_ChatService_Infrastructure.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace AWS_ChatService_Infrastructure.Services;

public class JwtService : IJwtService
{
    public string GenerateToken(User user, JwtConfigData jwtConfigData)
    {
        var tokenDescriptor = new SecurityTokenDescriptor();

        return "try";
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        throw new NotImplementedException();
    }
}