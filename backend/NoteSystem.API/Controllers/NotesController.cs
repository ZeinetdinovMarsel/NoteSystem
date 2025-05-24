using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NoteSystem.BusinessLogic.Services;
using NoteSystem.Core.Dtos;
using NoteSystem.Core.Interfaces;

namespace NoteSystem.API.Controllers;
[ApiController]
[Route("[controller]")]
public class NotesController : ControllerBase
{
    private readonly INoteService _noteService;
    private readonly ICryptoService _cryptoService;
    private readonly IReminderService _reminderService;

    public NotesController(INoteService noteService, ICryptoService cryptoService, IReminderService reminderService)
    {
        _noteService = noteService;
        _cryptoService = cryptoService;
        _reminderService = reminderService;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<NoteDto>>> GetAll()
    {
        var userIdClaim = User.FindFirst("userId");
        var userId = Guid.Parse(userIdClaim.Value);

        var notes = await _noteService.GetAllNotesAsync(userId);

        var decryptedNotes = notes
                .Select(n => new NoteDto(
                    n.NoteId,
                    n.CategoryId,
                    n.UserId,
                    _cryptoService.Decrypt(n.Title), 
                    _cryptoService.Decrypt(n.Content),
                    n.ReminderDate
                ))
            .ToList();
        return Ok(decryptedNotes);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<NoteDto>> GetById(Guid id)
    {
        var userIdClaim = User.FindFirst("userId");
        var userId = Guid.Parse(userIdClaim.Value);
        var note = await _noteService.GetNoteByIdAsync(id, userId);

        note = note with
        {
            Title = _cryptoService.Decrypt(note.Title),
            Content = _cryptoService.Decrypt(note.Content)
        };

        return Ok(note);

    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Create([FromBody] NoteDto createDto)
    {
        var userIdClaim = User.FindFirst("userId");

        var userId = Guid.Parse(userIdClaim.Value);

        createDto = createDto with { UserId = userId, Title = _cryptoService.Encrypt(createDto.Title), Content = _cryptoService.Encrypt(createDto.Content) };

        createDto = await _noteService.CreateNoteAsync(createDto);

        string? userEmail = User.FindFirst("userEmail")?.Value;

        if (createDto.ReminderDate.HasValue)
        {
            _reminderService.ScheduleReminder(createDto, userEmail);
        }
        return Ok(createDto);
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult> Update([FromBody] NoteDto updateDto)
    {

        var userIdClaim = User.FindFirst("userId");

        var userId = Guid.Parse(userIdClaim.Value);

        updateDto = updateDto with { UserId = userId, Title = _cryptoService.Encrypt(updateDto.Title), Content = _cryptoService.Encrypt(updateDto.Content) };

        string? userEmail = User.FindFirst("userEmail")?.Value;

        await _noteService.UpdateNoteAsync(updateDto);

        if (updateDto.ReminderDate.HasValue)
        {
            _reminderService.ScheduleReminder(updateDto, userEmail);
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _noteService.DeleteNoteAsync(id);
        return NoContent();
    }
}

