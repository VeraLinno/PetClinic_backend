namespace PetClinic.Application;

public interface IVisitService
{
    Task CompleteVisitAsync(Guid visitId, VisitCompletionDto dto);
}