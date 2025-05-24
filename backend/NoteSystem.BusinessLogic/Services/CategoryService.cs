using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using NoteSystem.Core.Dtos;
using NoteSystem.Core.Interfaces;
using Npgsql;

namespace NoteSystem.BusinessLogic.Services;
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid categoryId, Guid? userId = null)
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException("ID не может быть пустым.", nameof(categoryId));

        var category = await _categoryRepository.GetByIdAsync(categoryId, userId);
        if (category == null)
            throw new InvalidOperationException("Категория не найдена");

        return category;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(Guid? userId = null)
    {
        return await _categoryRepository.GetAllAsync(userId);
    }

    public async Task CreateCategoryAsync(CategoryDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.Name))
            throw new InvalidOperationException("Имя не должно быть пустым");

        if (await _categoryRepository.ExistsAsync(createDto.Name))
            throw new InvalidOperationException("Данная категория уже существует");


        await _categoryRepository.AddAsync(createDto);
        await _categoryRepository.SaveChangesAsync();
    }

    public async Task UpdateCategoryAsync(CategoryDto updateDto)
    {
        if (updateDto.CategoryId == Guid.Empty)
            throw new ArgumentException("ID не может быть пустым.", nameof(updateDto.CategoryId));

        if (string.IsNullOrWhiteSpace(updateDto.Name))
            throw new InvalidOperationException("Имя не должно быть пустым");

        if (await _categoryRepository.ExistsAsync(updateDto.Name, updateDto.CategoryId))
            throw new InvalidOperationException("Данная категория уже существует");

        await _categoryRepository.UpdateAsync(updateDto);
        await _categoryRepository.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException("ID не может быть пустым.", nameof(categoryId));

        if (!await _categoryRepository.ExistsAsync(categoryId))
            throw new InvalidOperationException("Категория не найдена");
        try
        {
            await _categoryRepository.DeleteAsync(categoryId);
            await _categoryRepository.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsForeignKeyViolation(ex))
        {
            throw new InvalidOperationException("Невозможно удалить категорию, так как существуют связанные заметки. Сначала удалите или переместите все заметки из этой категории.");
        }
    }

    private bool IsForeignKeyViolation(DbUpdateException ex)
    {
        return ex.InnerException is PostgresException pgEx &&
               pgEx.SqlState == "23503";
    }
}

