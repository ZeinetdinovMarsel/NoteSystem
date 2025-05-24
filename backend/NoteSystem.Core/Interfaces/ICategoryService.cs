using NoteSystem.Core.Dtos;

namespace NoteSystem.Core.Interfaces;
public interface ICategoryService
{
    Task CreateCategoryAsync(CategoryDto createDto);
    Task DeleteCategoryAsync(Guid categoryId);
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(Guid ?userId = null);
    Task<CategoryDto?> GetCategoryByIdAsync(Guid categoryId, Guid? userId = null);
    Task UpdateCategoryAsync(CategoryDto updateDto);
}