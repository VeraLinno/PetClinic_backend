using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetClinic.Application;
using PetClinic.Domain;

namespace PetClinic.Infrastructure;

public class TranslationService : ITranslationService
{
    private readonly PetClinicDbContext _dbContext;
    private readonly ILogger<TranslationService> _logger;

    public TranslationService(PetClinicDbContext dbContext, ILogger<TranslationService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<LanguageDictionaryDto> GetTranslationsByLanguageAsync(string languageCode)
    {
        _logger.LogInformation("Fetching translations for language: {LanguageCode}", languageCode);

        var translations = await _dbContext.Translations
            .Where(t => t.LanguageCode == languageCode && t.IsActive)
            .ToListAsync();

        var result = new LanguageDictionaryDto { LanguageCode = languageCode };

        foreach (var translation in translations)
        {
            if (!result.Translations.ContainsKey(translation.Category))
            {
                result.Translations[translation.Category] = new Dictionary<string, string>();
            }
            result.Translations[translation.Category][translation.Key] = translation.Value;
        }

        _logger.LogInformation("Fetched {Count} translations for language: {LanguageCode}",
            translations.Count, languageCode);

        return result;
    }

    public async Task<List<TranslationDto>> GetTranslationsByCategoryAsync(string languageCode, string category)
    {
        _logger.LogInformation("Fetching translations for language: {LanguageCode}, category: {Category}",
            languageCode, category);

        var translations = await _dbContext.Translations
            .Where(t => t.LanguageCode == languageCode && t.Category == category && t.IsActive)
            .Select(t => MapToDto(t))
            .ToListAsync();

        return translations;
    }

    public async Task<TranslationDto?> GetTranslationAsync(string languageCode, string category, string key)
    {
        var translation = await _dbContext.Translations
            .FirstOrDefaultAsync(t =>
                t.LanguageCode == languageCode &&
                t.Category == category &&
                t.Key == key);

        return translation != null ? MapToDto(translation) : null;
    }

    public async Task<List<TranslationDto>> SearchTranslationsAsync(TranslationFilterDto filter)
    {
        var query = _dbContext.Translations.AsQueryable();

        if (!string.IsNullOrEmpty(filter.LanguageCode))
            query = query.Where(t => t.LanguageCode == filter.LanguageCode);

        if (!string.IsNullOrEmpty(filter.Category))
            query = query.Where(t => t.Category == filter.Category);

        if (!string.IsNullOrEmpty(filter.Key))
            query = query.Where(t => t.Key.Contains(filter.Key));

        if (filter.IsActive.HasValue)
            query = query.Where(t => t.IsActive == filter.IsActive.Value);

        var translations = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(t => MapToDto(t))
            .ToListAsync();

        return translations;
    }

    public async Task<TranslationDto> CreateTranslationAsync(CreateTranslationDto dto)
    {
        _logger.LogInformation("Creating translation: {Key} for language: {LanguageCode}",
            dto.Key, dto.LanguageCode);

        // Check if translation already exists
        var existing = await _dbContext.Translations
            .FirstOrDefaultAsync(t =>
                t.Key == dto.Key &&
                t.LanguageCode == dto.LanguageCode &&
                t.Category == dto.Category);

        if (existing != null)
        {
            _logger.LogWarning("Translation already exists: {Key} for language: {LanguageCode}",
                dto.Key, dto.LanguageCode);
            throw new InvalidOperationException($"Translation with key '{dto.Key}' already exists for language '{dto.LanguageCode}'");
        }

        var translation = new Translation
        {
            Id = Guid.NewGuid(),
            Key = dto.Key,
            LanguageCode = dto.LanguageCode,
            Value = dto.Value,
            Category = dto.Category,
            Description = dto.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Translations.Add(translation);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Translation created successfully: {Id}", translation.Id);
        return MapToDto(translation);
    }

    public async Task<TranslationDto> UpdateTranslationAsync(Guid id, UpdateTranslationDto dto)
    {
        _logger.LogInformation("Updating translation: {Id}", id);

        var translation = await _dbContext.Translations.FindAsync(id);
        if (translation == null)
        {
            _logger.LogWarning("Translation not found: {Id}", id);
            throw new KeyNotFoundException($"Translation with id '{id}' not found");
        }

        translation.Value = dto.Value;
        translation.Description = dto.Description ?? translation.Description;
        translation.IsActive = dto.IsActive ?? translation.IsActive;
        translation.UpdatedAt = DateTime.UtcNow;

        _dbContext.Translations.Update(translation);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Translation updated successfully: {Id}", id);
        return MapToDto(translation);
    }

    public async Task DeleteTranslationAsync(Guid id)
    {
        _logger.LogInformation("Deleting translation: {Id}", id);

        var translation = await _dbContext.Translations.FindAsync(id);
        if (translation == null)
        {
            _logger.LogWarning("Translation not found: {Id}", id);
            throw new KeyNotFoundException($"Translation with id '{id}' not found");
        }

        _dbContext.Translations.Remove(translation);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Translation deleted successfully: {Id}", id);
    }

    public async Task<int> BulkCreateTranslationsAsync(List<CreateTranslationDto> translations)
    {
        _logger.LogInformation("Bulk creating {Count} translations", translations.Count);

        var newTranslations = translations.Select(dto => new Translation
        {
            Id = Guid.NewGuid(),
            Key = dto.Key,
            LanguageCode = dto.LanguageCode,
            Value = dto.Value,
            Category = dto.Category,
            Description = dto.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        _dbContext.Translations.AddRange(newTranslations);
        var count = await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Bulk created {Count} translations", count);
        return count;
    }

    public async Task<int> UpdateTranslationsAsync(List<(Guid Id, UpdateTranslationDto Dto)> updates)
    {
        _logger.LogInformation("Bulk updating {Count} translations", updates.Count);

        foreach (var (id, dto) in updates)
        {
            var translation = await _dbContext.Translations.FindAsync(id);
            if (translation != null)
            {
                translation.Value = dto.Value;
                translation.Description = dto.Description ?? translation.Description;
                translation.IsActive = dto.IsActive ?? translation.IsActive;
                translation.UpdatedAt = DateTime.UtcNow;
            }
        }

        var count = await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Bulk updated {Count} translations", count);
        return count;
    }

    public async Task<List<string>> GetAvailableLanguagesAsync()
    {
        var languages = await _dbContext.Translations
            .Select(t => t.LanguageCode)
            .Distinct()
            .OrderBy(l => l)
            .ToListAsync();

        return languages;
    }

    public async Task<List<string>> GetAvailableCategoriesAsync()
    {
        var categories = await _dbContext.Translations
            .Select(t => t.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

        return categories;
    }

    public async Task SeedTranslationsAsync(Dictionary<string, Dictionary<string, Dictionary<string, string>>> translations)
    {
        _logger.LogInformation("Seeding translations from JSON");

        foreach (var languageEntry in translations)
        {
            var languageCode = languageEntry.Key;
            var categories = languageEntry.Value;

            foreach (var categoryEntry in categories)
            {
                var category = categoryEntry.Key;
                var keys = categoryEntry.Value;

                foreach (var keyEntry in keys)
                {
                    var key = keyEntry.Key;
                    var value = keyEntry.Value;

                    // Check if translation already exists
                    var existing = await _dbContext.Translations
                        .FirstOrDefaultAsync(t =>
                            t.Key == key &&
                            t.LanguageCode == languageCode &&
                            t.Category == category);

                    if (existing == null)
                    {
                        var translation = new Translation
                        {
                            Id = Guid.NewGuid(),
                            Key = key,
                            LanguageCode = languageCode,
                            Value = value,
                            Category = category,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        };

                        _dbContext.Translations.Add(translation);
                    }
                }
            }
        }

        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Translations seeded successfully");
    }

    private static TranslationDto MapToDto(Translation translation)
    {
        return new TranslationDto
        {
            Id = translation.Id,
            Key = translation.Key,
            LanguageCode = translation.LanguageCode,
            Value = translation.Value,
            Category = translation.Category,
            Description = translation.Description,
            IsActive = translation.IsActive,
            CreatedAt = translation.CreatedAt,
            UpdatedAt = translation.UpdatedAt
        };
    }
}
