using NoteSystem.Core.Dtos;

public record CategoryReportDto(
    Guid CategoryId,
    string CategoryName,
    int TotalNotes,
    int NotesWithReminders,
    int CompletedReminders,
    int ActiveReminders,
    List<NoteDto> Notes
);

public record FullReportDto(
    List<CategoryReportDto> Categories,
    int TotalCategories,
    int TotalNotes,
    int TotalNotesWithReminders,
    int TotalCompletedReminders,
    int TotalActiveReminders
)
{
    public FullReportDto() : this(new List<CategoryReportDto>(), 0, 0, 0, 0, 0) { }
}