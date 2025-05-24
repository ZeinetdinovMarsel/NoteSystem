using NoteSystem.Core.Interfaces;

namespace NoteSystem.BusinessLogic.Services;
public class ReportService : IReportService
{
    private readonly INoteService _noteService;
    private readonly ICategoryService _categoryService;
    private readonly ICryptoService _cryptoService;

    public ReportService(INoteService noteService, ICategoryService categoryService, ICryptoService cryptoService)
    {
        _noteService = noteService;
        _categoryService = categoryService;
        _cryptoService = cryptoService;
    }

    public async Task<FullReportDto> GenerateReportAsync(Guid userId)
    {
        var categories = await _categoryService.GetAllCategoriesAsync(userId);
        var notes = await _noteService.GetAllNotesAsync(userId);

        var decryptedCategories = categories
                .Select(c => c with { Name = _cryptoService.Decrypt(c.Name) })
                .ToList();

        var categoryReports = new List<CategoryReportDto>();

        foreach (var category in decryptedCategories)
        {
            var categoryNotes = notes.Where(n => n.CategoryId == category.CategoryId).ToList();
            var decryptedNotes = categoryNotes
                .Select(n => n with { Title = _cryptoService.Decrypt(n.Title), Content = _cryptoService.Decrypt(n.Content) })
                .ToList();

            var notesWithReminders = categoryNotes.Count(n => n.ReminderDate.HasValue);
            var completedReminders = categoryNotes.Count(n => n.ReminderDate.HasValue && n.ReminderDate < DateTime.UtcNow);
            var activeReminders = notesWithReminders - completedReminders;

            categoryReports.Add(new CategoryReportDto(
                CategoryId: category.CategoryId,
                CategoryName: category.Name,
                TotalNotes: categoryNotes.Count,
                NotesWithReminders: notesWithReminders,
                CompletedReminders: completedReminders,
                ActiveReminders: activeReminders,
                Notes: decryptedNotes
            ));
        }

        return new FullReportDto(
            Categories: categoryReports,
            TotalCategories: categoryReports.Count,
            TotalNotes: categoryReports.Sum(c => c.TotalNotes),
            TotalNotesWithReminders: categoryReports.Sum(c => c.NotesWithReminders),
            TotalCompletedReminders: categoryReports.Sum(c => c.CompletedReminders),
            TotalActiveReminders: categoryReports.Sum(c => c.ActiveReminders)
        );
    }
}
