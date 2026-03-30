using Microsoft.AspNetCore.Http;
using PetClinic.Application;

namespace PetClinic.Infrastructure;

public class LocalizationService : ILocalizationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private static readonly Dictionary<string, Dictionary<string, string>> SpeciesTranslations = new(StringComparer.OrdinalIgnoreCase)
    {
        ["et"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Dog"] = "Koer",
            ["Cat"] = "Kass",
            ["Bird"] = "Lind",
            ["Fish"] = "Kala",
            ["Rabbit"] = "Küülik",
            ["Hamster"] = "Hamster",
            ["Reptile"] = "Roomaja",
            ["Other"] = "Muu"
        },
        ["ru"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Dog"] = "Собака",
            ["Cat"] = "Кошка",
            ["Bird"] = "Птица",
            ["Fish"] = "Рыба",
            ["Rabbit"] = "Кролик",
            ["Hamster"] = "Хомяк",
            ["Reptile"] = "Рептилия",
            ["Other"] = "Другое"
        }
    };

    private static readonly Dictionary<string, Dictionary<string, string>> BreedTranslations = new(StringComparer.OrdinalIgnoreCase)
    {
        ["et"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Golden Retriever"] = "Kuldne retriiver",
            ["Persian"] = "Pärsia",
            ["Mixed Breed"] = "Segatõug"
        },
        ["ru"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Golden Retriever"] = "Золотистый ретривер",
            ["Persian"] = "Персидская",
            ["Mixed Breed"] = "Смешанная порода"
        }
    };

    private static readonly Dictionary<string, Dictionary<string, string>> AppointmentStatusTranslations = new(StringComparer.OrdinalIgnoreCase)
    {
        ["et"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Pending"] = "Ootel",
            ["Confirmed"] = "Kinnitatud",
            ["Scheduled"] = "Planeeritud",
            ["Completed"] = "Lõpetatud",
            ["Cancelled"] = "Tühistatud"
        },
        ["ru"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Pending"] = "В ожидании",
            ["Confirmed"] = "Подтверждено",
            ["Scheduled"] = "Запланировано",
            ["Completed"] = "Завершено",
            ["Cancelled"] = "Отменено"
        }
    };

    private static readonly Dictionary<string, Dictionary<string, string>> VisitStatusTranslations = new(StringComparer.OrdinalIgnoreCase)
    {
        ["et"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Open"] = "Aktiivne",
            ["Completed"] = "Lõpetatud"
        },
        ["ru"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Open"] = "Открыт",
            ["Completed"] = "Завершен"
        }
    };

    public LocalizationService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetCurrentLanguage()
    {
        var header = _httpContextAccessor.HttpContext?.Request.Headers["Accept-Language"].ToString();
        if (string.IsNullOrWhiteSpace(header))
        {
            return "en";
        }

        var token = header.Split(',')[0].Trim();
        if (token.StartsWith("et", StringComparison.OrdinalIgnoreCase)) return "et";
        if (token.StartsWith("ru", StringComparison.OrdinalIgnoreCase)) return "ru";
        return "en";
    }

    public string LocalizePetSpecies(string species, string? language = null)
    {
        return LocalizeValue(species, language, SpeciesTranslations);
    }

    public string LocalizePetBreed(string breed, string? language = null)
    {
        return LocalizeValue(breed, language, BreedTranslations);
    }

    public string LocalizeAppointmentStatus(string status, string? language = null)
    {
        return LocalizeValue(status, language, AppointmentStatusTranslations);
    }

    public string LocalizeVisitStatus(string status, string? language = null)
    {
        return LocalizeValue(status, language, VisitStatusTranslations);
    }

    private string LocalizeValue(string value, string? language, Dictionary<string, Dictionary<string, string>> dictionary)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var locale = string.IsNullOrWhiteSpace(language) ? GetCurrentLanguage() : language;
        if (locale.Equals("en", StringComparison.OrdinalIgnoreCase))
        {
            return value;
        }

        if (dictionary.TryGetValue(locale, out var localeMap) && localeMap.TryGetValue(value, out var localized))
        {
            return localized;
        }

        return value;
    }
}
