using ERP.TRAN.CrossLayers.API.Pos.Shifts.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Terminals.Responses;
using ERP.TRAN.CrossLayers.API.Users.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.CajaService;

public partial class CajaManager
{
    /// <summary>
    /// Obtiene las cajas/terminales disponibles para un usuario según su rol y sucursales asignadas.
    /// Si es Cajero, solo retorna las cajas de sus sucursales asignadas (o todas si no tiene asignación explícita).
    /// Si es Admin o Supervisor, retorna todas las cajas activas.
    /// </summary>
    public async Task<List<PosTerminalDto>> GetAvailableTerminalsForUserAsync(int userId, UserRole role, CancellationToken cancellationToken = default)
    {
        var query = _context.PosTerminals
            .Include(t => t.Store)
            .Include(t => t.Warehouse)
            .Include(t => t.Shifts.Where(s => s.Status == PosShiftStatus.Open))
                .ThenInclude(s => s.Cajero)
            .Where(t => t.IsActive);

        if (role == UserRole.Cashier)
        {
            var assignedStoreIds = await _context.UsuarioStores
                .Where(us => us.UsuarioId == userId)
                .Select(us => us.StoreId)
                .ToListAsync(cancellationToken);

            if (assignedStoreIds.Any())
            {
                query = query.Where(t => assignedStoreIds.Contains(t.StoreId));
            }
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

    /// <summary>
    /// Consulta si el usuario ya tiene un turno abierto actualmente en alguna caja.
    /// </summary>
    public async Task<ActiveUserShiftDto?> GetActiveShiftForUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var activeShift = await _context.PosShifts
            .Include(s => s.PosTerminal)
                .ThenInclude(t => t.Store)
            .FirstOrDefaultAsync(s => s.CajeroId == userId && s.Status == PosShiftStatus.Open, cancellationToken);

        if (activeShift == null) return null;

        return new ActiveUserShiftDto(
            ShiftId: activeShift.Id,
            PosTerminalId: activeShift.PosTerminalId,
            TerminalName: activeShift.PosTerminal?.Name ?? $"Caja #{activeShift.PosTerminalId}",
            TerminalCode: activeShift.PosTerminal?.Code ?? "POS",
            StoreId: activeShift.PosTerminal?.StoreId ?? 0,
            StoreName: activeShift.PosTerminal?.Store?.Name ?? "Sucursal",
            OpenedAt: activeShift.OpenedAt,
            InitialCash: activeShift.InitialCash
        );
    }

    /// <summary>
    /// Lista todas las terminales registradas en el sistema para el panel de administración.
    /// </summary>
    public async Task<List<PosTerminalDto>> ListAllTerminalsAsync(int? storeId = null, CancellationToken cancellationToken = default)
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

    /// <summary>
    /// Lista los cajeros registrados en el sistema y las sucursales que tienen asignadas.
    /// </summary>
    public async Task<List<CashierAssignmentDto>> ListCashierStoreAssignmentsAsync(CancellationToken cancellationToken = default)
    {
        var cajeros = await _context.Usuarios
            .Where(u => u.Role == UserRole.Cashier && u.IsActive)
            .OrderBy(u => u.PrimerNombre)
            .ToListAsync(cancellationToken);

        var usuarioIds = cajeros.Select(c => c.Id).ToList();

        var asignaciones = await _context.UsuarioStores
            .Include(us => us.Store)
            .Where(us => usuarioIds.Contains(us.UsuarioId))
            .ToListAsync(cancellationToken);

        return cajeros.Select(u =>
        {
            var misTiendas = asignaciones
                .Where(a => a.UsuarioId == u.Id)
                .Select(a => new StoreAssignmentSummaryDto(
                    StoreId: a.StoreId,
                    StoreName: a.Store?.Name ?? $"Sucursal #{a.StoreId}",
                    IsDefault: a.IsDefault
                ))
                .ToList();

            var nombreCompleto = $"{u.PrimerNombre} {u.PrimerAPellido}".Trim();

            return new CashierAssignmentDto(
                UsuarioId: u.Id,
                NombreCompleto: string.IsNullOrEmpty(nombreCompleto) ? u.UserName : nombreCompleto,
                UserName: u.UserName,
                Email: u.Email,
                Role: u.Role,
                IsActive: u.IsActive,
                AssignedStores: misTiendas
            );
        }).ToList();
    }

    /// <summary>
    /// Asigna una sucursal a un cajero para que pueda abrir cajas en ella.
    /// </summary>
    public async Task AssignCashierToStoreAsync(int usuarioId, int storeId, bool isDefault, CancellationToken cancellationToken = default)
    {
        var exists = await _context.UsuarioStores
            .FirstOrDefaultAsync(us => us.UsuarioId == usuarioId && us.StoreId == storeId, cancellationToken);

        if (exists == null)
        {
            var nuevo = new UsuarioStore
            {
                UsuarioId = usuarioId,
                StoreId = storeId,
                IsDefault = isDefault
            };
            _context.UsuarioStores.Add(nuevo);
        }
        else
        {
            exists.IsDefault = isDefault;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Remueve la asignación de una sucursal para un cajero.
    /// </summary>
    public async Task RemoveCashierFromStoreAsync(int usuarioId, int storeId, CancellationToken cancellationToken = default)
    {
        var item = await _context.UsuarioStores
            .FirstOrDefaultAsync(us => us.UsuarioId == usuarioId && us.StoreId == storeId, cancellationToken);

        if (item != null)
        {
            _context.UsuarioStores.Remove(item);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
