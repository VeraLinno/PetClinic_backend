using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetClinic.Application;
using PetClinic.Domain;
using PetClinic.Infrastructure;

namespace PetClinic.Api.Controllers;

[ApiController]
[Route("api/v1/inventory")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly PetClinicDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContext;

    public InventoryController(PetClinicDbContext context, IMapper mapper, IUserContextService userContext)
    {
        _context = context;
        _mapper = mapper;
        _userContext = userContext;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        if (!IsCurrentUserVet())
        {
            return Forbid();
        }

        await ApplyDueReordersAsync();

        var items = await _context.MedicationStocks
            .OrderBy(m => m.Name)
            .ToListAsync();

        var dtos = _mapper.Map<List<MedicationStockDto>>(items);
        return Ok(dtos);
    }

    [HttpGet("low-stock")]
    [Authorize]
    public async Task<IActionResult> GetLowStock()
    {
        if (!IsCurrentUserVet())
        {
            return Forbid();
        }

        await ApplyDueReordersAsync();

        var lowStockItems = await _context.MedicationStocks
            .Where(m => m.Quantity <= m.ReorderLevel)
            .OrderBy(m => m.Name)
            .ToListAsync();

        var dtos = _mapper.Map<List<MedicationStockDto>>(lowStockItems);
        return Ok(dtos);
    }

    [HttpPut("{medicationId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateMedication(Guid medicationId, [FromBody] UpdateMedicationStockDto dto)
    {
        if (!IsCurrentUserVet())
        {
            return Forbid();
        }

        var normalizedName = dto.Name?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            return BadRequest("Medication name is required.");
        }

        if (dto.UnitPrice < 0)
        {
            return BadRequest("Unit price must be greater than or equal to zero.");
        }

        var medication = await _context.MedicationStocks
            .FirstOrDefaultAsync(m => m.Id == medicationId);

        if (medication == null)
        {
            return NotFound("Medication not found.");
        }

        medication.Name = normalizedName;
        medication.UnitPrice = dto.UnitPrice;

        await _context.SaveChangesAsync();

        var response = _mapper.Map<MedicationStockDto>(medication);
        return Ok(response);
    }

    [HttpPost("{medicationId:guid}/reorder")]
    [Authorize]
    public async Task<IActionResult> ReorderMedication(Guid medicationId, [FromBody] ReorderMedicationRequestDto dto)
    {
        if (!IsCurrentUserVet())
        {
            return Forbid();
        }

        if (dto.Quantity <= 0)
        {
            return BadRequest("Quantity must be greater than zero.");
        }

        var medication = await _context.MedicationStocks
            .FirstOrDefaultAsync(m => m.Id == medicationId);

        if (medication == null)
        {
            return NotFound("Medication not found.");
        }

        var deliveryAtUtc = DateTime.UtcNow.AddDays(1);
        var reorder = new InventoryReorder
        {
            MedicationStockId = medication.Id,
            Quantity = dto.Quantity,
            ScheduledForUtc = deliveryAtUtc,
            OrderedByVetId = _userContext.GetCurrentUserId()
        };

        _context.InventoryReorders.Add(reorder);
        await _context.SaveChangesAsync();

        var response = new ReorderMedicationResponseDto
        {
            MedicationId = medication.Id,
            MedicationName = medication.Name,
            OrderedQuantity = dto.Quantity,
            CurrentQuantity = medication.Quantity,
            DeliveryAtUtc = deliveryAtUtc,
            Message = $"Reorder placed. Package will arrive tomorrow at {deliveryAtUtc:HH:mm} UTC."
        };

        return Ok(response);
    }

    [HttpGet("incoming")]
    [Authorize]
    public async Task<IActionResult> GetIncomingReorders()
    {
        if (!IsCurrentUserVet())
        {
            return Forbid();
        }

        await ApplyDueReordersAsync();

        var currentUserId = _userContext.GetCurrentUserId();
        var pending = await _context.InventoryReorders
            .Include(r => r.MedicationStock)
            .Where(r => r.ReceivedAtUtc == null && r.OrderedByVetId == currentUserId)
            .OrderBy(r => r.ScheduledForUtc)
            .Select(r => new PendingInventoryReorderDto
            {
                ReorderId = r.Id,
                MedicationId = r.MedicationStockId,
                MedicationName = r.MedicationStock.Name,
                Quantity = r.Quantity,
                DeliveryAtUtc = r.ScheduledForUtc
            })
            .ToListAsync();

        return Ok(pending);
    }

    [HttpGet("delivered")]
    [Authorize]
    public async Task<IActionResult> GetDeliveredReorders()
    {
        if (!IsCurrentUserVet())
        {
            return Forbid();
        }

        await ApplyDueReordersAsync();

        var currentUserId = _userContext.GetCurrentUserId();
        var delivered = await _context.InventoryReorders
            .Include(r => r.MedicationStock)
            .Where(r => r.ReceivedAtUtc != null && r.OrderedByVetId == currentUserId)
            .OrderByDescending(r => r.ReceivedAtUtc)
            .Take(20)
            .Select(r => new DeliveredInventoryReorderDto
            {
                ReorderId = r.Id,
                MedicationId = r.MedicationStockId,
                MedicationName = r.MedicationStock.Name,
                Quantity = r.Quantity,
                DeliveryAtUtc = r.ScheduledForUtc,
                ReceivedAtUtc = r.ReceivedAtUtc!.Value
            })
            .ToListAsync();

        return Ok(delivered);
    }

    private async Task ApplyDueReordersAsync()
    {
        var nowUtc = DateTime.UtcNow;
        var dueReorders = await _context.InventoryReorders
            .Include(r => r.MedicationStock)
            .Where(r => r.ReceivedAtUtc == null && r.ScheduledForUtc <= nowUtc)
            .ToListAsync();

        if (dueReorders.Count == 0)
        {
            return;
        }

        foreach (var reorder in dueReorders)
        {
            reorder.MedicationStock.Quantity += reorder.Quantity;
            reorder.ReceivedAtUtc = nowUtc;
        }

        await _context.SaveChangesAsync();
    }

    private bool IsCurrentUserVet()
    {
        return _userContext.GetCurrentUserRoles().Contains("Vet");
    }
}
