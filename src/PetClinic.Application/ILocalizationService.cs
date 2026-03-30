namespace PetClinic.Application;

public interface ILocalizationService
{
    string GetCurrentLanguage();
    string LocalizePetSpecies(string species, string? language = null);
    string LocalizePetBreed(string breed, string? language = null);
    string LocalizeAppointmentStatus(string status, string? language = null);
    string LocalizeVisitStatus(string status, string? language = null);
}
