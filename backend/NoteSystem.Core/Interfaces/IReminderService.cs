using NoteSystem.Core.Dtos;

namespace NoteSystem.Core.Interfaces;
public interface IReminderService
{
    void ScheduleReminder(NoteDto note, string userEmail);
    Task SendReminderAsync(Guid noteId, string userEmail);
}