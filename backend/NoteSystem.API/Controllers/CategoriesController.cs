using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NoteSystem.Core.Dtos;
using NoteSystem.Core.Interfaces;
using Org.BouncyCastle.Utilities;

namespace NoteSystem.API.Controllers;
[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ICryptoService _cryptoService;
    public CategoriesController(ICategoryService categoryService, ICryptoService cryptoService)
    {
        _categoryService = categoryService;
        _cryptoService = cryptoService;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var userIdClaim = User.FindFirst("userId");
        var userId = Guid.Parse(userIdClaim.Value);
        var categories = await _categoryService.GetAllCategoriesAsync(userId);

        var decryptedCategories = categories
                .Select(c => new CategoryDto(
                    c.CategoryId,
                    _cryptoService.Decrypt(c.Name),
                    c.UserId
                ))
            .ToList();

        return Ok(decryptedCategories);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id)
    {
        var userIdClaim = User.FindFirst("userId");
        var userId = Guid.Parse(userIdClaim.Value);
        var category = await _categoryService.GetCategoryByIdAsync(id, userId);


        category = category with
        {
            Name = _cryptoService.Decrypt(category.Name),
        };
        return Ok(category);

    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Create([FromBody] CategoryDto createDto)
    {
        var userIdClaim = User.FindFirst("userId");
        if (userIdClaim == null)
            return Unauthorized("Пользователь не авторизован");

        var userId = Guid.Parse(userIdClaim.Value);

        createDto = createDto with {Name = _cryptoService.Encrypt(createDto.Name), UserId = userId };

        await _categoryService.CreateCategoryAsync(createDto);
        return Ok(createDto);
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult> Update([FromBody] CategoryDto updateDto)
    {
        updateDto = updateDto with { Name = _cryptoService.Encrypt(updateDto.Name) };
        await _categoryService.UpdateCategoryAsync(updateDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return NoContent();
    }
}

