using NoteSystem.Core.Dtos;

namespace NoteSystem.Core.Interfaces;
public interface INoteRepository
{
    Task<NoteDto?> AddAsync(NoteDto createDto);
    Task DeleteAsync(Guid noteId);
    Task<bool> ExistsAsync(Guid noteId);
    Task<bool> ExistsAsync(string noteTitle, Guid? noteId = null);
    Task<IEnumerable<NoteDto>> GetAllAsync(Guid? userId = null);
    Task<NoteDto?> GetByIdAsync(Guid noteId, Guid? userId = null);
    Task SaveChangesAsync();
    Task UpdateAsync(NoteDto note);
}