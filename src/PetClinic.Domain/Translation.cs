namespace PetClinic.Domain;

/// <summary>
/// Represents a translatable string in the system
/// Supports multiple languages per translation key
/// </summary>
public class Translation : BaseEntity
{
    /// <summary>
    /// Unique key for the translation (e.g., "common.ok", "auth.email")
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// Language code (en, et, ru, etc.)
    /// </summary>
    public required string LanguageCode { get; set; }

    /// <summary>
    /// Translated value for this key in the specified language
    /// </summary>
    public required string Value { get; set; }

    /// <summary>
    /// Category/namespace for grouping translations (e.g., "common", "auth", "appointments")
    /// </summary>
    public required string Category { get; set; }

    /// <summary>
    /// Optional description for translator context
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indicates if this translation is actively used
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Date when translation was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date when translation was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
