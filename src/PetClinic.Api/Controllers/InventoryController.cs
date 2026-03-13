using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetClinic.Application;

namespace PetClinic.Api.Controllers;

[ApiController]
[Route("api/v1/inventory")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly PetClinicDbContext _context;
    private readonly IMapper _mapper;

    public InventoryController(PetClinicDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet("low-stock")]
    [Authorize(Roles = "Vet")]
    public async Task<IActionResult> GetLowStock()
    {
        var lowStockItems = await _context.MedicationStocks
            .Where(m => m.Quantity < 10) // Threshold for low stock
            .ToListAsync();

        var dtos = _mapper.Map<List<MedicationStockDto>>(lowStockItems);
        return Ok(dtos);
    }
}