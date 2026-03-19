using AutoMapper;
using PetClinic.Domain;

namespace PetClinic.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Owner, OwnerDto>();
        CreateMap<Pet, PetDto>();
        CreateMap<CreatePetDto, Pet>();
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(d => d.PetName, opt => opt.MapFrom(s => s.Pet.Name))
            .ForMember(
                d => d.VeterinarianName,
                opt => opt.MapFrom(s => s.Veterinarian != null ? $"{s.Veterinarian.Name} {s.Veterinarian.LastName}".Trim() : null)
            );
        CreateMap<Visit, VisitDto>();
        CreateMap<Invoice, InvoiceDto>();
        CreateMap<MedicationStock, MedicationStockDto>();
        CreateMap<Prescription, PrescriptionDto>();
    }
}