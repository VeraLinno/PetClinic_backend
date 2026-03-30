namespace PetClinic.Application;

public interface ITranslationService
{
    // Get translations by language for frontend
    Task<LanguageDictionaryDto> GetTranslationsByLanguageAsync(string languageCode);

    // Get all translations in a category
    Task<List<TranslationDto>> GetTranslationsByCategoryAsync(string languageCode, string category);

    // Get a specific translation
    Task<TranslationDto?> GetTranslationAsync(string languageCode, string category, string key);

    // Search translations
    Task<List<TranslationDto>> SearchTranslationsAsync(TranslationFilterDto filter);

    // CRUD operations
    Task<TranslationDto> CreateTranslationAsync(CreateTranslationDto dto);
    Task<TranslationDto> UpdateTranslationAsync(Guid id, UpdateTranslationDto dto);
    Task DeleteTranslationAsync(Guid id);

    // Bulk operations
    Task<int> BulkCreateTranslationsAsync(List<CreateTranslationDto> translations);
    Task<int> UpdateTranslationsAsync(List<(Guid Id, UpdateTranslationDto Dto)> updates);

    // Get available languages
    Task<List<string>> GetAvailableLanguagesAsync();

    // Get available categories
    Task<List<string>> GetAvailableCategoriesAsync();

    // Seed translations from JSON
    Task SeedTranslationsAsync(Dictionary<string, Dictionary<string, Dictionary<string, string>>> translations);
}
