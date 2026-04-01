using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Asp.Versioning;
using PetClinic.Application;
using PetClinic.Domain;
using PetClinic.Infrastructure;

namespace PetClinic.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class OwnersController : ControllerBase
{
    private readonly PetClinicDbContext _context;
    private readonly IUserContextService _userContext;
    private readonly IMapper _mapper;
    private readonly ILocalizationService _localizationService;

    public OwnersController(PetClinicDbContext context, IUserContextService userContext, IMapper mapper, ILocalizationService localizationService)
    {
        _context = context;
        _userContext = userContext;
        _mapper = mapper;
        _localizationService = localizationService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = _userContext.GetCurrentUserId();
        var owner = await _context.Owners.FirstOrDefaultAsync(o => o.Id == userId);
        if (owner == null) return NotFound();
        var dto = _mapper.Map<OwnerDto>(owner);
        return Ok(dto);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateOwnerProfileDto dto)
    {
        var userId = _userContext.GetCurrentUserId();
        var owner = await _context.Owners.FirstOrDefaultAsync(o => o.Id == userId);
        if (owner == null) return NotFound();

        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        var emailInUse = await _context.Owners.AnyAsync(o => o.Id != userId && o.Email.ToLower() == normalizedEmail);
        if (emailInUse)
        {
            return Conflict(new { message = "Email is already in use." });
        }

        owner.Email = normalizedEmail;
        owner.FirstName = string.IsNullOrWhiteSpace(dto.FirstName) ? null : dto.FirstName.Trim();
        owner.LastName = string.IsNullOrWhiteSpace(dto.LastName) ? null : dto.LastName.Trim();

        await _context.SaveChangesAsync();

        var result = _mapper.Map<OwnerDto>(owner);
        return Ok(result);
    }

    [HttpGet("me/pets")]
    public async Task<IActionResult> GetMyPets()
    {
        var userId = _userContext.GetCurrentUserId();
        var language = _localizationService.GetCurrentLanguage();
        var pets = await _context.Pets
            .Where(p => p.OwnerId == userId)
            .ToListAsync();
        var dtos = _mapper.Map<List<PetDto>>(pets);
        ApplyPetLocalization(dtos, language);
        return Ok(dtos);
    }

    [HttpGet("vets")]
    public async Task<IActionResult> GetVeterinarians()
    {
        var vets = await _context.Veterinarians
            .Select(v => new VeterinarianOptionDto
            {
                Id = v.Id,
                Name = ((v.Name ?? string.Empty) + " " + (v.LastName ?? string.Empty)).Trim(),
                Email = v.Email
            })
            .ToListAsync();

        foreach (var vet in vets.Where(v => string.IsNullOrWhiteSpace(v.Name)))
        {
            vet.Name = vet.Email;
        }

        return Ok(vets);
    }

    [HttpGet("pets")]
    public async Task<IActionResult> GetAllPetsForVet()
    {
        var roles = _userContext.GetCurrentUserRoles();
        if (!roles.Contains("Vet") && !roles.Contains("Admin"))
        {
            return Forbid();
        }

        var language = _localizationService.GetCurrentLanguage();
        var pets = await _context.Pets
            .OrderBy(p => p.Name)
            .ToListAsync();

        var dtos = _mapper.Map<List<PetDto>>(pets);
        ApplyPetLocalization(dtos, language);
        return Ok(dtos);
    }

    [HttpPost("me/pets")]
    public async Task<IActionResult> CreatePet([FromBody] CreatePetDto dto)
    {
        var userId = _userContext.GetCurrentUserId();
        var language = _localizationService.GetCurrentLanguage();
        var owner = await _context.Owners.FirstOrDefaultAsync(o => o.Id == userId);
        if (owner == null) return NotFound();

        var pet = _mapper.Map<Pet>(dto);
        pet.OwnerId = userId;

        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();

        var result = _mapper.Map<PetDto>(pet);
        ApplyPetLocalization(result, language);
        return Created($"api/v1/owners/me/pets/{pet.Id}", result);
    }

    [HttpPut("me/pets/{petId:guid}")]
    public async Task<IActionResult> UpdatePet(Guid petId, [FromBody] CreatePetDto dto)
    {
        var userId = _userContext.GetCurrentUserId();
        var language = _localizationService.GetCurrentLanguage();
        var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == petId && p.OwnerId == userId);
        if (pet == null) return NotFound();

        pet.Name = dto.Name;
        pet.Species = dto.Species;
        pet.Breed = dto.Breed;
        pet.DateOfBirth = dto.DateOfBirth;

        await _context.SaveChangesAsync();

        var result = _mapper.Map<PetDto>(pet);
        ApplyPetLocalization(result, language);
        return Ok(result);
    }

    private void ApplyPetLocalization(IEnumerable<PetDto> pets, string language)
    {
        foreach (var pet in pets)
        {
            ApplyPetLocalization(pet, language);
        }
    }

    private void ApplyPetLocalization(PetDto pet, string language)
    {
        pet.SpeciesLocalized = _localizationService.LocalizePetSpecies(pet.Species, language);
        pet.BreedLocalized = _localizationService.LocalizePetBreed(pet.Breed, language);
    }

    [HttpDelete("me/pets/{petId:guid}")]
    public async Task<IActionResult> DeletePet(Guid petId)
    {
        var userId = _userContext.GetCurrentUserId();
        var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == petId && p.OwnerId == userId);
        if (pet == null) return NotFound();

        _context.Pets.Remove(pet);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
