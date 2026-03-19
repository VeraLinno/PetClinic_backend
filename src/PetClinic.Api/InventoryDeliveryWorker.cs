using Microsoft.EntityFrameworkCore;
using PetClinic.Infrastructure;

public class InventoryDeliveryWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<InventoryDeliveryWorker> _logger;

    public InventoryDeliveryWorker(IServiceScopeFactory scopeFactory, ILogger<InventoryDeliveryWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<PetClinicDbContext>();

                var nowUtc = DateTime.UtcNow;
                var dueReorders = await dbContext.InventoryReorders
                    .Include(r => r.MedicationStock)
                    .Where(r => r.ReceivedAtUtc == null && r.ScheduledForUtc <= nowUtc)
                    .ToListAsync(stoppingToken);

                if (dueReorders.Count > 0)
                {
                    foreach (var reorder in dueReorders)
                    {
                        reorder.MedicationStock.Quantity += reorder.Quantity;
                        reorder.ReceivedAtUtc = nowUtc;
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("Applied {Count} scheduled inventory deliveries.", dueReorders.Count);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to apply scheduled inventory deliveries.");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
