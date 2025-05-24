using NoteSystem.Core.Dtos;

namespace NoteSystem.Core.Interfaces;
public interface INoteService
{
    Task<NoteDto> CreateNoteAsync(NoteDto createDto);
    Task DeleteNoteAsync(Guid noteId);
    Task<IEnumerable<NoteDto>> GetAllNotesAsync(Guid? userId = null);
    Task<NoteDto?> GetNoteByIdAsync(Guid noteId, Guid? userId = null);
    Task UpdateNoteAsync(NoteDto updateDto);
}