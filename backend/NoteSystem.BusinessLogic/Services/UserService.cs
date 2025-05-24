using NoteSystem.Core.Dtos;
using NoteSystem.Core.Interfaces;
using NoteSystem.BusinessLogic.Validator;

namespace NoteSystem.BusinessLogic.Services;
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly UserValidator _userValidator;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtProvider jwtProvider, UserValidator userValidator)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _jwtProvider = jwtProvider ?? throw new ArgumentNullException(nameof(jwtProvider));
        _userValidator = userValidator ?? throw new ArgumentNullException(nameof(userValidator));
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("ID не может быть пустым.", nameof(userId));

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException("Пользователь не найден");

        return user;
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Почта не может быть пустой", nameof(email));

        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            throw new InvalidOperationException("Пользователь не найден");

        return user;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<string> LoginAsync(LoginUserDto loginDto)
    {
        if (loginDto == null)
            throw new ArgumentNullException(nameof(loginDto));

        UserDto userDto = await _userValidator.ValidateLoginUserAsync(loginDto);

        if (!_passwordHasher.Verify(loginDto.Password, userDto.Password))
            throw new UnauthorizedAccessException("Неверный пароль");

        var token = _jwtProvider.GenerateToken(userDto);
        return token;
    }

    public async Task CreateUserAsync(UserDto createDto)
    {
        await _userValidator.ValidateCreateUserAsync(createDto);

        var hashedPassword = _passwordHasher.Generate(createDto.Password);

        createDto = createDto with { Password = hashedPassword };

        await _userRepository.AddAsync(createDto);
        await _userRepository.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(UserDto updateDto)
    {
        await _userValidator.ValidateUpdateUserAsync(updateDto);

        var hashedPassword = _passwordHasher.Generate(updateDto.Password);

        updateDto = updateDto with { Password = hashedPassword };

        await _userRepository.UpdateAsync(updateDto);
        await _userRepository.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("ID не может быть пустым.", nameof(userId));

        if (!await _userRepository.ExistsAsync(userId))
            throw new InvalidOperationException("Пользователь не найден");

        await _userRepository.DeleteAsync(userId);
        await _userRepository.SaveChangesAsync();
    }
}