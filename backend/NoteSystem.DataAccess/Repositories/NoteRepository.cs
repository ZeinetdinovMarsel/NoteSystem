using Microsoft.EntityFrameworkCore;
using NoteSystem.Core.Dtos;
using NoteSystem.Core.Interfaces;
using NoteSystem.DataAccess.Entities;

namespace NoteSystem.DataAccess.Repositories;
public class NoteRepository : INoteRepository
{
    private readonly NoteSystemDbContext _context;

    public NoteRepository(NoteSystemDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<NoteDto>> GetAllAsync(Guid? userId = null)
    {
        var notes = await _context.Notes
            .Where(n => n.UserId == userId)
            .ToListAsync();

        return notes.Select(MapToDto);
    }

    public async Task<NoteDto?> GetByIdAsync(Guid noteId, Guid? userId = null)
    {
        var note = await _context.Notes
            .Where(n => n.UserId == userId)
            .FirstOrDefaultAsync(n => n.NoteId == noteId);

        return note != null ? MapToDto(note) : null;
    }

    public async Task<NoteDto> AddAsync(NoteDto createDto)
    {
        if (createDto == null)
            throw new ArgumentNullException(nameof(createDto));

        var note = new NoteEntity
        {
            NoteId = Guid.NewGuid(),
            CategoryId = createDto.CategoryId,
            UserId = createDto.UserId,
            Title = createDto.Title,
            Content = createDto.Content,
            ReminderDate = createDto.ReminderDate
        };

        await _context.Notes.AddAsync(note);

        return MapToDto(note);
    }

    public async Task UpdateAsync(NoteDto note)
    {
        if (note == null)
            throw new ArgumentNullException(nameof(note));

        await _context.Notes
            .Where(n => n.NoteId == note.NoteId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(n => n.CategoryId, note.CategoryId)
                .SetProperty(n => n.Title, note.Title)
                .SetProperty(n => n.Content, note.Content)
                .SetProperty(n => n.ReminderDate, note.ReminderDate));
    }

    public async Task DeleteAsync(Guid noteId)
    {
        var note = await _context.Notes.FindAsync(noteId);
        if (note != null)
        {
            _context.Notes.Remove(note);
        }
    }

    public async Task<bool> ExistsAsync(Guid noteId)
    {
        return await _context.Notes.AnyAsync(n => n.NoteId == noteId);
    }

    public async Task<bool> ExistsAsync(string noteTitle, Guid? noteId = null)
    {
        return await _context.Notes.AnyAsync(n => n.Title == noteTitle && (!noteId.HasValue || n.NoteId != noteId.Value));
    }


    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    private NoteDto MapToDto(NoteEntity note)
    {
        return new NoteDto(note.NoteId, note.CategoryId, note.UserId, note.Title, note.Content, note.ReminderDate);
    }
}
