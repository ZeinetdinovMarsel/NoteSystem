using Microsoft.EntityFrameworkCore;
using NoteSystem.Core.Dtos;
using NoteSystem.DataAccess.Entities;
using NoteSystem.Core.Interfaces;
namespace NoteSystem.DataAccess.Repositories;
public class UserRepository : IUserRepository
{
    private readonly NoteSystemDbContext _context;

    public UserRepository(NoteSystemDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<UserDto?> GetByIdAsync(Guid userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        return user != null ? MapToDto(user) : null;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _context.Users.ToListAsync();

        return users.Select(MapToDto);
    }

    public async Task AddAsync(UserDto createDto)
    {
        if (createDto == null)
            throw new ArgumentNullException(nameof(createDto));

        var user = new UserEntity
        {
            UserId = Guid.NewGuid(),
            UserName = createDto.UserName,
            Email = createDto.Email,
            PasswordHash = createDto.Password
        };

        await _context.Users.AddAsync(user);
    }

    public async Task UpdateAsync(UserDto user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        await _context.Users
            .Where(u => u.UserId == user.UserId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(u => u.UserName, user.UserName)
                .SetProperty(u => u.Email, user.Email)
                .SetProperty(u => u.PasswordHash, user.Password));
    }

    public async Task DeleteAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            _context.Users.Remove(user);
        }
    }

    public async Task<bool> ExistsAsync(Guid userId)
    {
        return await _context.Users.AnyAsync(u => u.UserId == userId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    private UserDto MapToDto(UserEntity user)
    {
        return new UserDto(user.UserId, user.UserName, user.Email, user.PasswordHash);
    }
}