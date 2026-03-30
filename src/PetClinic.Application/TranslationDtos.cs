namespace PetClinic.Application;

public class TranslationDto
{
    public Guid Id { get; set; }
    public required string Key { get; set; }
    public required string LanguageCode { get; set; }
    public required string Value { get; set; }
    public required string Category { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateTranslationDto
{
    public required string Key { get; set; }
    public required string LanguageCode { get; set; }
    public required string Value { get; set; }
    public required string Category { get; set; }
    public string? Description { get; set; }
}

public class UpdateTranslationDto
{
    public required string Value { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}

public class TranslationFilterDto
{
    public string? LanguageCode { get; set; }
    public string? Category { get; set; }
    public string? Key { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class LanguageDictionaryDto
{
    public required string LanguageCode { get; set; }
    public Dictionary<string, Dictionary<string, string>> Translations { get; set; } = new();
}
