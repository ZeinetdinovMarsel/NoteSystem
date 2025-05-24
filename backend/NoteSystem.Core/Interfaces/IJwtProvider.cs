using NoteSystem.Core.Dtos;

namespace NoteSystem.Core.Interfaces;
public interface IJwtProvider
{
    string GenerateToken(UserDto user);
    Guid ValidateToken(string token);
}