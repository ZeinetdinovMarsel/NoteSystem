using System.ComponentModel.DataAnnotations;

namespace NoteSystem.DataAccess.Entities;
public class UserEntity
{
    [Key]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;


    public ICollection<CategoryEntity> Categories { get; set; } = new List<CategoryEntity>();
    public ICollection<NoteEntity> Notes { get; set; } = new List<NoteEntity>();
}
