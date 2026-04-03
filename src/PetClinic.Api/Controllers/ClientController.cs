using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetClinic.Application;
using PetClinic.Domain;
using PetClinic.Infrastructure;

namespace PetClinic.Api.Controllers;

[Authorize(Policy = "Owner")]
[Route("client")]
public class ClientController : Controller
{
    private readonly PetClinicDbContext _context;
    private readonly IUserContextService _userContext;
    private readonly IAppointmentService _appointmentService;
    private readonly ILocalizationService _localizationService;
    private readonly ITranslationService _translationService;
    private readonly IMapper _mapper;

    public ClientController(
        PetClinicDbContext context,
        IUserContextService userContext,
        IAppointmentService appointmentService,
        ILocalizationService localizationService,
        ITranslationService translationService,
        IMapper mapper)
    {
        _context = context;
        _userContext = userContext;
        _appointmentService = appointmentService;
        _localizationService = localizationService;
        _translationService = translationService;
        _mapper = mapper;
    }

    [HttpGet("")]
    [HttpGet("index")]
    public async Task<IActionResult> Index()
    {
        var userId = _userContext.GetCurrentUserId();
        await PopulateUiTextsAsync();

        var owner = await _context.Owners
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == userId);

        var petsCount = await _context.Pets.CountAsync(p => p.OwnerId == userId);
        var appointmentsCount = await _context.Appointments
            .CountAsync(a => a.Pet.OwnerId == userId && a.Status != AppointmentStatus.Cancelled);
        var invoicesCount = await _context.Invoices
            .CountAsync(i => i.Visit.Appointment.Pet.OwnerId == userId);

        ViewBag.OwnerName = owner?.FirstName ?? owner?.Email ?? "Owner";
        ViewBag.PetsCount = petsCount;
        ViewBag.AppointmentsCount = appointmentsCount;
        ViewBag.InvoicesCount = invoicesCount;

        return View();
    }

    [HttpGet("pets")]
    public async Task<IActionResult> Pets()
    {
        var userId = _userContext.GetCurrentUserId();
        var language = _localizationService.GetCurrentLanguage();
        await PopulateUiTextsAsync();

        var pets = await _context.Pets
            .Where(p => p.OwnerId == userId)
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync();

        var dtos = _mapper.Map<List<PetDto>>(pets);
        foreach (var pet in dtos)
        {
            pet.SpeciesLocalized = _localizationService.LocalizePetSpecies(pet.Species, language);
            pet.BreedLocalized = _localizationService.LocalizePetBreed(pet.Breed, language);
        }

        return View(dtos);
    }

    [HttpGet("appointments")]
    public async Task<IActionResult> Appointments()
    {
        var userId = _userContext.GetCurrentUserId();
        var roles = _userContext.GetCurrentUserRoles();
        var language = _localizationService.GetCurrentLanguage();
        await PopulateUiTextsAsync();

        var appointments = await _appointmentService.GetUserAppointmentsAsync(userId, roles);
        var dtos = _mapper.Map<List<AppointmentDto>>(appointments);

        foreach (var appointment in dtos)
        {
            appointment.StatusLocalized = _localizationService.LocalizeAppointmentStatus(appointment.Status, language);
        }

        return View(dtos);
    }

    [HttpGet("invoices")]
    public async Task<IActionResult> Invoices()
    {
        var userId = _userContext.GetCurrentUserId();
        await PopulateUiTextsAsync();

        var invoices = await _context.Invoices
            .Where(i => i.Visit.Appointment.Pet.OwnerId == userId)
            .AsNoTracking()
            .OrderByDescending(i => i.IssuedAt)
            .Select(i => new InvoiceDto
            {
                Id = i.Id,
                VisitId = i.VisitId,
                Amount = i.Amount,
                IssuedAt = i.IssuedAt,
                Status = i.Status.ToString(),
                PaidAt = i.PaidAt,
                DueDate = i.DueDate
            })
            .ToListAsync();

        return View(invoices);
    }

    [HttpPost("invoices/{id:guid}/mark-paid")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkInvoicePaid(Guid id)
    {
        var userId = _userContext.GetCurrentUserId();

        var invoice = await _context.Invoices
            .Include(i => i.Visit)
                .ThenInclude(v => v.Appointment)
                    .ThenInclude(a => a.Pet)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null)
            return NotFound();

        if (invoice.Visit.Appointment.Pet.OwnerId != userId)
            return Forbid();

        if (invoice.Status != Invoice.PaymentStatus.Paid)
        {
            invoice.Status = Invoice.PaymentStatus.Paid;
            invoice.PaidAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        TempData["Success"] = "Invoice marked as paid.";
        return RedirectToAction(nameof(Invoices));
    }

    private async Task PopulateUiTextsAsync()
    {
        var language = _localizationService.GetCurrentLanguage();
        var uiTexts = await _translationService.GetTranslationsByCategoryAsync(language, "mvc.client");
        ViewBag.UiTexts = uiTexts.ToDictionary(t => t.Key, t => t.Value, StringComparer.OrdinalIgnoreCase);
    }
}
