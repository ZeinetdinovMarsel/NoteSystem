using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoteSystem.DataAccess.Entities;
public class NoteEntity
{
    [Key]
    public Guid NoteId { get; set; }

    [Required]
    public Guid CategoryId  { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime? ReminderDate { get; set; }

    [ForeignKey("CategoryId")]
    public CategoryEntity? Category { get; set; }

    [ForeignKey("UserId")]
    public UserEntity? User { get; set; }
}
