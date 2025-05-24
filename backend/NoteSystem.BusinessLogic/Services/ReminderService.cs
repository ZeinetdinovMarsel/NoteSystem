using Hangfire;
using NoteSystem.Core.Dtos;
using NoteSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteSystem.BusinessLogic.Services;
public class ReminderService : IReminderService
{
    private readonly INoteService _noteService;
    private readonly IEmailService _emailService;
    private readonly ICryptoService _cryptoService;
    private readonly IUserService _userService;

    public ReminderService(INoteService noteService, IEmailService emailService, ICryptoService cryptoService, IUserService userService)
    {
        _noteService = noteService;
        _emailService = emailService;
        _cryptoService = cryptoService;
        _userService = userService;
    }

    public void ScheduleReminder(NoteDto note, string userEmail)
    {
        if (note.ReminderDate.HasValue)
        {
            BackgroundJob.Schedule(() => SendReminderAsync(note.NoteId, userEmail), note.ReminderDate.Value);
        }
    }

    public async Task SendReminderAsync(Guid noteId, string userEmail)
    {

        var user = await _userService.GetUserByEmailAsync(userEmail);
        var note = await _noteService.GetNoteByIdAsync(noteId, user?.UserId);
        if (note == null) return;

        var decryptedTitle = _cryptoService.Decrypt(note.Title);
        var decryptedContent = _cryptoService.Decrypt(note.Content);
        var subject = $"Напоминание: {decryptedTitle}";
        var body = $"<p>Это напоминание о заметке: <strong>{decryptedTitle}</strong></p><p>Содержимое: {decryptedContent}</p>";

        userEmail = _cryptoService.Decrypt(userEmail);
        await _emailService.SendEmailAsync(userEmail, subject, body);
    }
}