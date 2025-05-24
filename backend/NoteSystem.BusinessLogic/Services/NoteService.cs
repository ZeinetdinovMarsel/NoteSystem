using Microsoft.AspNetCore.Http.HttpResults;
using NoteSystem.Core.Dtos;
using NoteSystem.Core.Interfaces;

namespace NoteSystem.BusinessLogic.Services;
public class NoteService : INoteService
{
    private readonly INoteRepository _noteRepository;
    private readonly ICategoryRepository _categoryRepository;

    public NoteService(INoteRepository noteRepository, ICategoryRepository categoryRepository)
    {
        _noteRepository = noteRepository;
        _categoryRepository = categoryRepository;

    }

    public async Task<NoteDto?> GetNoteByIdAsync(Guid noteId, Guid? userId = null)
    {
        if (noteId == Guid.Empty)
            throw new ArgumentException("ID не может быть пустым.", nameof(noteId));

        var note = await _noteRepository.GetByIdAsync(noteId, userId);

        if (note == null)
            throw new InvalidOperationException("Заметка не найдена");

        return note;
    }

    public async Task<IEnumerable<NoteDto>> GetAllNotesAsync(Guid? userId = null)
    {
        return await _noteRepository.GetAllAsync(userId);
    }

    public async Task<NoteDto?> CreateNoteAsync(NoteDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto?.Title))
            throw new InvalidOperationException("Название не должно быть пустым");

        if (string.IsNullOrWhiteSpace(createDto?.Content))
            throw new InvalidOperationException("Описание не должно быть пустым");

        if (await _noteRepository.ExistsAsync(createDto.Title))
            throw new InvalidOperationException("Данная заметка уже существует");

        if (DateTime.UtcNow > (createDto.ReminderDate ?? DateTime.UtcNow))
            throw new InvalidOperationException("Время напоминания должно быть позже текущего времени");

        var category = await _categoryRepository.GetByIdAsync(createDto.CategoryId, createDto.UserId);

        if (category?.UserId != createDto.UserId)
        {
            throw new InvalidOperationException("Данная категория не принадлежит вам");
        }

        NoteDto note = await _noteRepository.AddAsync(createDto) ?? createDto;
        await _noteRepository.SaveChangesAsync();

        return note;
    }

    public async Task UpdateNoteAsync(NoteDto updateDto)
    {
        if (updateDto.NoteId == Guid.Empty)
            throw new ArgumentException("ID не может быть пустым.", nameof(updateDto.NoteId));

        if (string.IsNullOrWhiteSpace(updateDto?.Title))
            throw new InvalidOperationException("Название не должно быть пустым");

        if (string.IsNullOrWhiteSpace(updateDto?.Content))
            throw new InvalidOperationException("Описание не должно быть пустым");

        if (await _noteRepository.ExistsAsync(updateDto.Title, updateDto.NoteId))
            throw new InvalidOperationException("Данная заметка уже существует");

        if (DateTime.UtcNow > (updateDto.ReminderDate ?? DateTime.UtcNow))
            throw new InvalidOperationException("Время напоминания должно быть позже текущего времени");

        var category = await _categoryRepository.GetByIdAsync(updateDto.CategoryId, updateDto.UserId);

        if (category?.UserId != updateDto.UserId)
        {
            throw new InvalidOperationException("Данная категория не принадлежит вам");
        }

        await _noteRepository.UpdateAsync(updateDto);
        await _noteRepository.SaveChangesAsync();
    }

    public async Task DeleteNoteAsync(Guid noteId)
    {
        if (noteId == Guid.Empty)
            throw new ArgumentException("ID не может быть пустым.", nameof(noteId));

        if (!await _noteRepository.ExistsAsync(noteId))
            throw new InvalidOperationException("Категория не найдена");

        await _noteRepository.DeleteAsync(noteId);
        await _noteRepository.SaveChangesAsync();
    }
}

