using AutoMapper;
using PetClinic.Domain;

namespace PetClinic.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Owner, OwnerDto>();
        CreateMap<Pet, PetDto>();
        CreateMap<Appointment, AppointmentDto>();
        CreateMap<Visit, VisitDto>();
        CreateMap<Invoice, InvoiceDto>();
        CreateMap<MedicationStock, MedicationStockDto>();
        CreateMap<Prescription, PrescriptionDto>();
    }
}