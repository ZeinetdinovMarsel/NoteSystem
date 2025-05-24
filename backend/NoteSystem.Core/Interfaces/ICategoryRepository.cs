using NoteSystem.Core.Dtos;

namespace NoteSystem.Core.Interfaces;
public interface ICategoryRepository
{
    Task AddAsync(CategoryDto createDto);
    Task DeleteAsync(Guid categoryId);
    Task<bool> ExistsAsync(Guid categoryId);
    Task<bool> ExistsAsync(string categoryName, Guid? categoryId = null);
    Task<IEnumerable<CategoryDto>> GetAllAsync(Guid? userId = null);
    Task<CategoryDto?> GetByIdAsync(Guid categoryId, Guid? userId = null);
    Task UpdateAsync(CategoryDto category);
    Task SaveChangesAsync();
}