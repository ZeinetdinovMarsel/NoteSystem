using System.ComponentModel.DataAnnotations;

namespace NoteSystem.Core.Dtos;
public record UserDto(Guid UserId, [Required] string UserName, [Required] string Email, [Required] string Password);
public record LoginUserDto([Required] string Email, [Required] string Password);