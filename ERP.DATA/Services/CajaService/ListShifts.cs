using ERP.TRAN.CrossLayers.API.Pos.Shifts.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Shifts.Responses;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.CajaService;

public partial class CajaManager
{
    /// <summary>
    /// Lista turnos cerrados (cierres de caja) para auditoría, con filtros y paginación.
    /// </summary>
    public async Task<PagedList<PosShiftSummaryDto>> ListClosedShiftsAsync(
        DateTime? fromDate,
        DateTime? toDate,
        int? terminalId,
        int? cashierId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 20;

        var query = _context.PosShifts
            .AsNoTracking()
            .Where(s => s.Status == PosShiftStatus.Closed);

        if (fromDate.HasValue)
        {
            var desde = fromDate.Value.Kind == DateTimeKind.Utc ? fromDate.Value : fromDate.Value.ToUniversalTime();
            query = query.Where(s => s.OpenedAt >= desde);
        }
        if (toDate.HasValue)
        {
            var hasta = toDate.Value.Kind == DateTimeKind.Utc ? toDate.Value : toDate.Value.ToUniversalTime();
            // Incluir el día completo si llega solo fecha
            if (toDate.Value.TimeOfDay == TimeSpan.Zero)
                hasta = hasta.AddDays(1);
            query = query.Where(s => s.OpenedAt < hasta);
        }
        if (terminalId.HasValue && terminalId.Value > 0)
            query = query.Where(s => s.PosTerminalId == terminalId.Value);
        if (cashierId.HasValue && cashierId.Value > 0)
            query = query.Where(s => s.CajeroId == cashierId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var rows = await query
            .OrderByDescending(s => s.ClosedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new
            {
                s.Id,
                s.PosTerminalId,
                TerminalName = s.PosTerminal != null ? s.PosTerminal.Name : "",
                TerminalCode = s.PosTerminal != null ? s.PosTerminal.Code : "",
                CashierName = s.Cajero != null ? (s.Cajero.PrimerNombre + " " + s.Cajero.PrimerAPellido).Trim() : "",
                s.OpenedAt,
                s.ClosedAt,
                s.InitialCash,
                s.CashSales,
                s.CardSales,
                s.TransferSales,
                s.CreditSales,
                s.TotalExpectedCash,
                s.ActualCash,
                s.Difference,
                s.Status,
                TotalTransactions = s.Sales != null ? s.Sales.Count : 0
            })
            .ToListAsync(cancellationToken);

        var items = rows.Select(r => new PosShiftSummaryDto(
            Id: r.Id,
            PosTerminalId: r.PosTerminalId,
            TerminalName: r.TerminalName,
            TerminalCode: r.TerminalCode,
            CashierName: r.CashierName,
            OpenedAt: r.OpenedAt,
            ClosedAt: r.ClosedAt,
            InitialCash: r.InitialCash,
            CashSales: r.CashSales,
            CardSales: r.CardSales,
            TransferSales: r.TransferSales,
            CreditSales: r.CreditSales,
            TotalSales: r.CashSales + r.CardSales + r.TransferSales + r.CreditSales,
            TotalExpectedCash: r.TotalExpectedCash,
            ActualCash: r.ActualCash,
            Difference: r.Difference,
            Status: r.Status,
            TotalTransactions: r.TotalTransactions
        )).ToList();

        return new PagedList<PosShiftSummaryDto>(items, totalCount, pageNumber, pageSize);
    }

    /// <summary>
    /// Obtiene el detalle completo de un turno (cerrado o abierto) para auditoría.
    /// </summary>
    public async Task<PosShiftDetailDto?> GetShiftDetailAsync(int shiftId, CancellationToken cancellationToken = default)
    {
        var turno = await _context.PosShifts
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == shiftId, cancellationToken);
        if (turno == null) return null;

        var terminalInfo = await _context.PosTerminals
            .Where(t => t.Id == turno.PosTerminalId)
            .Select(t => new { t.Name, t.Code })
            .FirstOrDefaultAsync(cancellationToken);

        var cajeroNombre = await _context.Usuarios
            .Where(u => u.Id == turno.CajeroId)
            .Select(u => u.PrimerNombre + " " + u.PrimerAPellido)
            .FirstOrDefaultAsync(cancellationToken);

        var movementTotals = await _context.PosShiftMovements
            .Where(m => m.PosShiftId == turno.Id)
            .GroupBy(m => m.Type)
            .Select(g => new { Type = g.Key, Total = g.Sum(m => m.Amount) })
            .ToListAsync(cancellationToken);

        decimal totalIncomes = movementTotals.FirstOrDefault(x => x.Type == PosShiftMovementType.Income)?.Total ?? 0m;
        decimal totalExpenses = movementTotals.FirstOrDefault(x => x.Type == PosShiftMovementType.Expense)?.Total ?? 0m;

        var ventas = await _context.Sales
            .Where(s => s.PosShiftId == turno.Id)
            .Select(sale => new SaleSummaryDto(
                Id: sale.Id,
                SaleNumber: sale.SaleNumber,
                CreatedAt: sale.CreatedAt,
                ClientName: sale.Client.Name,
                WarehouseName: sale.Warehouse.Name,
                Subtotal: sale.Subtotal,
                TaxAmount: sale.TaxAmount,
                Total: sale.Total,
                StatusDisplay: sale.Status.ToString(),
                PaymentStatusDisplay: sale.PaymentStatus.ToString(),
                LineCount: sale.Lines.Count,
                FactusStatus: sale.FactusStatus,
                FactusInvoiceNumber: sale.FactusInvoiceNumber
            ))
            .ToListAsync(cancellationToken);

        return new PosShiftDetailDto(
            Id: turno.Id,
            PosTerminalId: turno.PosTerminalId,
            TerminalName: terminalInfo?.Name ?? string.Empty,
            TerminalCode: terminalInfo?.Code ?? string.Empty,
            CashierId: turno.CajeroId.ToString(),
            CashierName: cajeroNombre ?? string.Empty,
            OpenedAt: turno.OpenedAt,
            ClosedAt: turno.ClosedAt,
            InitialCash: turno.InitialCash,
            CashSales: turno.CashSales,
            CardSales: turno.CardSales,
            TransferSales: turno.TransferSales,
            CreditSales: turno.CreditSales,
            TotalSales: turno.CashSales + turno.CardSales + turno.TransferSales + turno.CreditSales,
            CashWithdrawals: totalExpenses,
            CashAdditions: totalIncomes,
            TotalExpectedCash: turno.TotalExpectedCash,
            ActualCash: turno.ActualCash,
            Difference: turno.Difference,
            Status: turno.Status,
            Notes: turno.Notes,
            Sales: ventas
        );
    }
}
