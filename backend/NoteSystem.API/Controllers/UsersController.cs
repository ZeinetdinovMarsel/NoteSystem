using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NoteSystem.Core.Dtos;
using NoteSystem.Core.Interfaces;
using Org.BouncyCastle.Utilities;
using System.Security.Claims;

namespace NoteSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICryptoService _cryptoService;

        public UsersController(IUserService userService, ICryptoService cryptoService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _cryptoService = cryptoService;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            string? userEmail = User.FindFirst("userEmail")?.Value ?? throw new InvalidOperationException("Почта пользователя не найдена");
            var user = await _userService.GetUserByEmailAsync(userEmail);
            user = user with { UserName = _cryptoService.Decrypt(user.UserName), Email = _cryptoService.Decrypt(user.Email) };

            return Ok(user);
        }

        [HttpGet("{userId}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetUserById(Guid userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            user = user with { UserName = _cryptoService.Decrypt(user.UserName), Email = _cryptoService.Decrypt(user.Email) };

            return Ok(user);
        }

        [HttpGet("email/{email}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);

            user = user with { UserName = _cryptoService.Decrypt(user.UserName), Email = _cryptoService.Decrypt(user.Email) };
            return Ok(user);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            var decryptedUsers = users
               .Select(u => new UserDto(
                   u.UserId,
                    _cryptoService.Decrypt(u.UserName),
                    _cryptoService.Decrypt(u.Email),
                   u.Password
               ))
           .ToList();
            return Ok(users);
        }

        [HttpPost("register")]
        public async Task<ActionResult> CreateUser([FromBody] UserDto createDto)
        {
            createDto = createDto with { UserName = _cryptoService.Encrypt(createDto.UserName), Email = _cryptoService.Encrypt(createDto.Email) };
            await _userService.CreateUserAsync(createDto);
            return Ok(createDto);
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginUser([FromBody] LoginUserDto loginDto)
        {
            loginDto = loginDto with { Email = _cryptoService.Encrypt(loginDto.Email) };
            string jwtToken = await _userService.LoginAsync(loginDto);
            return Ok(jwtToken);
        }
        [HttpPut("{userId}")]
        [Authorize]
        public async Task<ActionResult> UpdateUser(Guid userId, [FromBody] UserDto updateDto)
        {
            updateDto = updateDto with { UserName = _cryptoService.Encrypt(updateDto.UserName), Email = _cryptoService.Encrypt(updateDto.Email) };
            await _userService.UpdateUserAsync(updateDto);
            return NoContent();
        }

        [HttpDelete("{userId}")]
        [Authorize]
        public async Task<ActionResult> DeleteUser(Guid userId)
        {
            await _userService.DeleteUserAsync(userId);
            return NoContent();
        }
    }
}