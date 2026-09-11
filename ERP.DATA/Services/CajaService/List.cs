using ERP.TRAN.CrossLayers.API.Pos.Shifts.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Terminals.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.CajaService;

public partial class CajaManager
{    /// <summary>
    /// Lista todas las terminales registradas en el sistema para el panel de administración.
    /// </summary>
    public async Task<List<PosTerminalDto>> List(int? storeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.PosTerminals
            .Include(t => t.Store)
            .Include(t => t.Warehouse)
            .Include(t => t.Shifts.Where(s => s.Status == PosShiftStatus.Open))
            .ThenInclude(s => s.Cajero)
            .AsQueryable();

        if (storeId.HasValue && storeId.Value > 0)
        {
            query = query.Where(t => t.StoreId == storeId.Value);
        }

        var terminals = await query
            .OrderBy(t => t.Store.Name)
            .ThenBy(t => t.Name)
            .ToListAsync(cancellationToken);

        return terminals.Select(t =>
        {
            var activeShift = t.Shifts?.FirstOrDefault(s => s.Status == PosShiftStatus.Open);
            var cashierName = activeShift?.Cajero != null 
                ? $"{activeShift.Cajero.PrimerNombre} {activeShift.Cajero.PrimerAPellido}".Trim() 
                : null;

            return new PosTerminalDto(
                Id: t.Id,
                Name: t.Name,
                Code: t.Code,
                StoreId: t.StoreId,
                StoreName: t.Store?.Name ?? "Sin Tienda",
                WarehouseId: t.WarehouseId,
                WarehouseName: t.Warehouse?.Name ?? "Sin Almacén",
                Prefix: t.Prefix,
                CurrentConsecutive: t.CurrentConsecutive,
                DianResolutionNumber: t.DianResolutionNumber,
                IsActive: t.IsActive,
                HasActiveShift: activeShift != null,
                ActiveShiftId: activeShift?.Id,
                ActiveCashierName: cashierName
            );
        }).ToList();
    }
}