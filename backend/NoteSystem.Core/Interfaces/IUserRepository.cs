using NoteSystem.Core.Dtos;

namespace NoteSystem.Core.Interfaces;
public interface IUserRepository
{
    Task AddAsync(UserDto createDto);
    Task DeleteAsync(Guid userId);
    Task<bool> ExistsAsync(Guid userId);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByEmailAsync(string email);
    Task<UserDto?> GetByIdAsync(Guid userId);
    Task SaveChangesAsync();
    Task UpdateAsync(UserDto user);
}