using System.ComponentModel.DataAnnotations;

namespace NoteSystem.Core.Dtos;
public record CategoryDto([Required] Guid CategoryId, [Required] string Name, [Required] Guid UserId);