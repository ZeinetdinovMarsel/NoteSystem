using System.ComponentModel.DataAnnotations;

namespace NoteSystem.Core.Dtos;
public record NoteDto([Required] Guid NoteId,[Required] Guid CategoryId,Guid UserId, [Required] string Title,[Required] string Content, DateTime? ReminderDate = null);