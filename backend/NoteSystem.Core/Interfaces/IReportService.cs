
namespace NoteSystem.Core.Interfaces;

public interface IReportService
{
    Task<FullReportDto> GenerateReportAsync(Guid userId);
}