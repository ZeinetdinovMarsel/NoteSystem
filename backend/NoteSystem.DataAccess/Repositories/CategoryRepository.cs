using Microsoft.EntityFrameworkCore;
using NoteSystem.Core.Dtos;
using NoteSystem.Core.Interfaces;
using NoteSystem.DataAccess.Entities;

namespace NoteSystem.DataAccess.Repositories;
public class CategoryRepository : ICategoryRepository
{
    private readonly NoteSystemDbContext _context;

    public CategoryRepository(NoteSystemDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync(Guid? userId = null)
    {
        var categories = await _context.Categories
            .Where(c => c.UserId == userId)
            .ToListAsync();

        return categories.Select(MapToDto);
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid categoryId, Guid? userId = null)
    {
        var category = await _context.Categories
            .Where(c => c.UserId == userId)
            .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

        return category != null ? MapToDto(category) : null;
    }

    public async Task AddAsync(CategoryDto createDto)
    {
        if (createDto == null)
            throw new ArgumentNullException(nameof(createDto));

        var category = new CategoryEntity
        {
            CategoryId = Guid.NewGuid(),
            Name = createDto.Name,
            UserId = createDto.UserId
        };

        await _context.Categories.AddAsync(category);
    }

    public async Task UpdateAsync(CategoryDto category)
    {
        if (category == null)
            throw new ArgumentNullException(nameof(category));

        await _context.Categories
            .Where(c => c.CategoryId == category.CategoryId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(c => c.Name, category.Name));
    }

    public async Task DeleteAsync(Guid categoryId)
    {
        var category = await _context.Categories.FindAsync(categoryId);
        if (category != null)
        {
            _context.Categories.Remove(category);
        }
    }

    public async Task<bool> ExistsAsync(Guid categoryId)
    {
        return await _context.Categories.AnyAsync(c => c.CategoryId == categoryId);
    }

    public async Task<bool> ExistsAsync(string categoryName, Guid? categoryId = null)
    {
        return await _context.Categories.AnyAsync(c => c.Name == categoryName && (!categoryId.HasValue || c.CategoryId != categoryId.Value));
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    private CategoryDto MapToDto(CategoryEntity category)
    {
        return new CategoryDto(category.CategoryId, category.Name, category.UserId);
    }
}
