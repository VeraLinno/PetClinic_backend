using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using PetClinic.Application;

namespace PetClinic.Api.Controllers.Admin;

/// <summary>
/// Admin finance controller - manage invoices and financial reports
/// </summary>
[ApiController]
[Area("Admin")]
[Route("admin/[controller]")]
[ApiVersion("1.0")]
[Authorize(Policy = "Admin")]
public class AdminFinanceController : Controller
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminFinanceController> _logger;

    public AdminFinanceController(IAdminService adminService, ILogger<AdminFinanceController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    /// <summary>
    /// GET: /admin/adminfinance - Display invoices list
    /// </summary>
    [HttpGet("index")]
    [HttpGet("")]
    public async Task<IActionResult> Index(DateTime? fromDate = null, DateTime? toDate = null)
    {
        _logger.LogInformation("Admin: Finance/Invoices page accessed (from: {FromDate}, to: {ToDate})", fromDate, toDate);

        try
        {
            var invoices = await _adminService.GetAllInvoicesAsync(fromDate, toDate);
            return View("~/Views/Admin/Finance/Index.cshtml", invoices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading invoices list");
            ModelState.AddModelError("Error", "Failed to load invoices");
            return View("~/Views/Admin/Finance/Index.cshtml", new List<InvoiceDto>());
        }
    }

    /// <summary>
    /// GET: /admin/adminfinance/reports - Display financial reports
    /// </summary>
    [HttpGet("reports")]
    public async Task<IActionResult> Reports(int months = 1)
    {
        _logger.LogInformation("Admin: Financial reports accessed (months: {Months})", months);

        try
        {
            var toDate = DateTime.UtcNow;
            var fromDate = toDate.AddMonths(-months);

            var report = await _adminService.GetFinancialReportAsync(fromDate, toDate);
            return View("~/Views/Admin/Finance/Reports.cshtml", report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading financial report");
            ModelState.AddModelError("Error", "Failed to load financial report");
            return View("~/Views/Admin/Finance/Reports.cshtml");
        }
    }

    /// <summary>
    /// GET: /admin/adminfinance/invoices/id - Display invoice details
    /// </summary>
    [HttpGet("invoices/{id:guid}")]
    public async Task<IActionResult> InvoiceDetails(Guid id)
    {
        _logger.LogInformation("Admin: Invoice details accessed for {InvoiceId}", id);

        try
        {
            var invoices = await _adminService.GetAllInvoicesAsync();
            var invoice = invoices.FirstOrDefault(i => i.Id == id);

            if (invoice == null)
                return NotFound();

            return View("~/Views/Admin/Finance/Adjust.cshtml", invoice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading invoice details for {InvoiceId}", id);
            return NotFound();
        }
    }

    /// <summary>
    /// GET: /admin/adminfinance/adjust/id - Display adjust invoice form
    /// </summary>
    [HttpGet("adjust/{id:guid}")]
    public async Task<IActionResult> Adjust(Guid id)
    {
        _logger.LogInformation("Admin: Adjust invoice form accessed for {InvoiceId}", id);

        try
        {
            var invoices = await _adminService.GetAllInvoicesAsync();
            var invoice = invoices.FirstOrDefault(i => i.Id == id);

            if (invoice == null)
                return NotFound();

            return View("~/Views/Admin/Finance/Adjust.cshtml", invoice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading adjust invoice form for {InvoiceId}", id);
            return NotFound();
        }
    }

    /// <summary>
    /// POST: /admin/adminfinance/adjust/id - Adjust invoice amount
    /// </summary>
    [HttpPost("adjust/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdjustConfirmed(Guid id, [FromForm] decimal newAmount, [FromForm] string reason)
    {
        _logger.LogWarning("Admin: Adjusting invoice {InvoiceId} to {NewAmount} - Reason: {Reason}", id, newAmount, reason);

        try
        {
            await _adminService.AdjustInvoiceAsync(id, newAmount, reason);
            TempData["Success"] = "Invoice adjusted successfully";
            return RedirectToAction("InvoiceDetails", new { id = id });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Invoice not found: {InvoiceId}", id);
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adjusting invoice {InvoiceId}", id);
            ModelState.AddModelError("Error", "Failed to adjust invoice");
            var invoices = await _adminService.GetAllInvoicesAsync();
            var invoice = invoices.FirstOrDefault(i => i.Id == id);
            return View("~/Views/Admin/Finance/Adjust.cshtml", invoice);
        }
    }

    /// <summary>
    /// GET: /admin/adminfinance/api/invoices - Get all invoices (API)
    /// </summary>
    [HttpGet("api/invoices")]
    [Produces("application/json")]
    public async Task<IActionResult> GetAllInvoices(DateTime? fromDate = null, DateTime? toDate = null)
    {
        try
        {
            var invoices = await _adminService.GetAllInvoicesAsync(fromDate, toDate);
            return Ok(invoices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving invoices");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// GET: /admin/adminfinance/api/report - Get financial report (API)
    /// </summary>
    [HttpGet("api/report")]
    [Produces("application/json")]
    public async Task<IActionResult> GetFinancialReport(int months = 1)
    {
        try
        {
            var toDate = DateTime.UtcNow;
            var fromDate = toDate.AddMonths(-months);

            var report = await _adminService.GetFinancialReportAsync(fromDate, toDate);
            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving financial report");
            return BadRequest(new { error = ex.Message });
        }
    }
}
