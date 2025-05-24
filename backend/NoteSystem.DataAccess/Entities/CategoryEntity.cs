using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoteSystem.DataAccess.Entities;
public class CategoryEntity
{
    [Key]
    public Guid CategoryId { get; set; }
    
    [Required]
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public UserEntity? User { get; set; }
    public ICollection<NoteEntity> Notes { get; set; } = new List<NoteEntity>();
}
