using Microsoft.AspNetCore.Http.HttpResults;
using NoteSystem.Core.Dtos;
using NoteSystem.Core.Interfaces;
using System.Text.RegularExpressions;

namespace NoteSystem.BusinessLogic.Validator;
public class UserValidator
{
    private readonly IUserRepository _userRepository;
    private readonly ICryptoService _cryptoService;

    public UserValidator(IUserRepository userRepository, ICryptoService cryptoService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _cryptoService = cryptoService;
    }

    public async Task ValidateCreateUserAsync(UserDto createDto)
    {
        createDto = createDto with { UserName = _cryptoService.Decrypt(createDto.UserName), Email = _cryptoService.Decrypt(createDto.Email) };

        if (createDto == null)
            throw new ArgumentNullException(nameof(createDto), "Поля не могут быть пустыми.");

        ValidateUserName(createDto.UserName);
        ValidateEmailAsync(createDto.Email);


        var existingUser = await _userRepository.GetByEmailAsync(createDto.Email);
        if (existingUser != null)
            throw new InvalidOperationException("Почта уже используется");

        ValidatePassword(createDto.Password);
    }

    public async Task ValidateUpdateUserAsync(UserDto updateDto)
    {
        updateDto = updateDto with { UserName = _cryptoService.Decrypt(updateDto.UserName), Email = _cryptoService.Decrypt(updateDto.Email) };

        if (updateDto == null)
            throw new ArgumentNullException(nameof(updateDto), "Поля не могут быть пустыми.");
        if (!await _userRepository.ExistsAsync(updateDto.UserId))
            throw new InvalidOperationException("Пользователь не найден");

        ValidateUserName(updateDto.UserName);
        ValidateEmailAsync(updateDto.Email, updateDto.UserId);


        var existingUser = await _userRepository.GetByEmailAsync(updateDto.Email);

        if (existingUser != null)
            throw new InvalidOperationException("Почта уже используется");

        ValidatePassword(updateDto.Password);
    }
    
    public async Task<UserDto> ValidateLoginUserAsync(LoginUserDto updateDto)
    {

        if (updateDto == null)
            throw new ArgumentNullException(nameof(updateDto), "Поля не могут быть пустыми.");

        LoginUserDto formatValidate = updateDto with {Email = _cryptoService.Decrypt(updateDto.Email) };
        
        ValidateEmailAsync(formatValidate.Email);
        ValidatePassword(formatValidate.Password);

        var existingUser = await _userRepository.GetByEmailAsync(updateDto.Email);
        if (existingUser == null)
            throw new InvalidOperationException("Пользователя с данной почтой не существует");

        return existingUser;
        
    }

    private void ValidateUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Имя пользователя не может быть пустым", nameof(userName));

        if (userName.Length > 50)
            throw new ArgumentException("Имя пользователя не может превышать 50 символов", nameof(userName));

        if (!Regex.IsMatch(userName, @"^[a-zA-Z0-9_]+$"))
            throw new ArgumentException("Имя пользователя может содержать только буквы, цифры и нижние подчёркивания.", nameof(userName));
    }

    private void ValidateEmailAsync(string email, Guid? excludeUserId = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Почта не может быть пустой", nameof(email));

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new ArgumentException("Неверный формат почты", nameof(email));
    }

    private void ValidatePassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Пароль не может быть пустым", nameof(passwordHash));
    }
}
