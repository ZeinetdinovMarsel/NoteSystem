using NoteSystem.Core.Dtos;

namespace NoteSystem.Core.Interfaces;
public interface IUserService
{
    Task CreateUserAsync(UserDto createDto);
    Task DeleteUserAsync(Guid userId);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<UserDto?> GetUserByIdAsync(Guid userId);
    Task UpdateUserAsync(UserDto updateDto);
    Task<string> LoginAsync(LoginUserDto loginDto);
}