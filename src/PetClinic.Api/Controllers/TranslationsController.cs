using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using PetClinic.Application;

namespace PetClinic.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
public class TranslationsController : ControllerBase
{
    private readonly ITranslationService _translationService;
    private readonly ILogger<TranslationsController> _logger;

    public TranslationsController(ITranslationService translationService, ILogger<TranslationsController> logger)
    {
        _translationService = translationService;
        _logger = logger;
    }

    /// <summary>
    /// Get all translations for a language (Public - for frontend)
    /// </summary>
    [HttpGet("language/{languageCode}")]
    [AllowAnonymous]
    public async Task<ActionResult<LanguageDictionaryDto>> GetTranslationsByLanguage(string languageCode)
    {
        try
        {
            var translations = await _translationService.GetTranslationsByLanguageAsync(languageCode);
            return Ok(translations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching translations for language: {LanguageCode}", languageCode);
            return StatusCode(500, new { message = "Error fetching translations" });
        }
    }

    /// <summary>
    /// Get translations by category
    /// </summary>
    [HttpGet("language/{languageCode}/category/{category}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<TranslationDto>>> GetTranslationsByCategory(string languageCode, string category)
    {
        try
        {
            var translations = await _translationService.GetTranslationsByCategoryAsync(languageCode, category);
            return Ok(translations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching translations for language: {LanguageCode}, category: {Category}",
                languageCode, category);
            return StatusCode(500, new { message = "Error fetching translations" });
        }
    }

    /// <summary>
    /// Get a specific translation
    /// </summary>
    [HttpGet("language/{languageCode}/category/{category}/key/{key}")]
    [AllowAnonymous]
    public async Task<ActionResult<TranslationDto>> GetTranslation(string languageCode, string category, string key)
    {
        try
        {
            var translation = await _translationService.GetTranslationAsync(languageCode, category, key);
            if (translation == null)
                return NotFound();

            return Ok(translation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching translation: {LanguageCode}/{Category}/{Key}",
                languageCode, category, key);
            return StatusCode(500, new { message = "Error fetching translation" });
        }
    }

    /// <summary>
    /// Get available languages
    /// </summary>
    [HttpGet("languages")]
    [AllowAnonymous]
    public async Task<ActionResult<List<string>>> GetAvailableLanguages()
    {
        try
        {
            var languages = await _translationService.GetAvailableLanguagesAsync();
            return Ok(languages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching available languages");
            return StatusCode(500, new { message = "Error fetching languages" });
        }
    }

    /// <summary>
    /// Get available categories
    /// </summary>
    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<ActionResult<List<string>>> GetAvailableCategories()
    {
        try
        {
            var categories = await _translationService.GetAvailableCategoriesAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching available categories");
            return StatusCode(500, new { message = "Error fetching categories" });
        }
    }

    /// <summary>
    /// Search translations (Admin only)
    /// </summary>
    [HttpPost("search")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<List<TranslationDto>>> SearchTranslations([FromBody] TranslationFilterDto filter)
    {
        try
        {
            var translations = await _translationService.SearchTranslationsAsync(filter);
            return Ok(translations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching translations");
            return StatusCode(500, new { message = "Error searching translations" });
        }
    }

    /// <summary>
    /// Create a new translation (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<TranslationDto>> CreateTranslation([FromBody] CreateTranslationDto dto)
    {
        try
        {
            var translation = await _translationService.CreateTranslationAsync(dto);
            return CreatedAtAction(nameof(GetTranslation),
                new { languageCode = translation.LanguageCode, category = translation.Category, key = translation.Key },
                translation);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid translation creation attempt");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating translation");
            return StatusCode(500, new { message = "Error creating translation" });
        }
    }

    /// <summary>
    /// Update a translation (Admin only)
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<TranslationDto>> UpdateTranslation(Guid id, [FromBody] UpdateTranslationDto dto)
    {
        try
        {
            var translation = await _translationService.UpdateTranslationAsync(id, dto);
            return Ok(translation);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Translation not found: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating translation: {Id}", id);
            return StatusCode(500, new { message = "Error updating translation" });
        }
    }

    /// <summary>
    /// Delete a translation (Admin only)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> DeleteTranslation(Guid id)
    {
        try
        {
            await _translationService.DeleteTranslationAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Translation not found: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting translation: {Id}", id);
            return StatusCode(500, new { message = "Error deleting translation" });
        }
    }

    /// <summary>
    /// Bulk create translations (Admin only)
    /// </summary>
    [HttpPost("bulk")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<int>> BulkCreateTranslations([FromBody] List<CreateTranslationDto> translations)
    {
        try
        {
            var count = await _translationService.BulkCreateTranslationsAsync(translations);
            return Ok(new { createdCount = count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk creating translations");
            return StatusCode(500, new { message = "Error bulk creating translations" });
        }
    }
}
