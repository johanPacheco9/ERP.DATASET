using ERP.TRAN.CrossLayers.API.Pos.Sales.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Responses;
using ERP.TRAN.CrossLayers.API.Pos.Shifts.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Shifts.Requests;
using ERP.TRAN.CrossLayers.API.Pos.Shifts.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.CajaService;

public partial class CajaManager
{
    public async Task<PosShiftDetailDto> CierreCaja(CloseShiftRequest request, CancellationToken cancellationToken)
    {
        // 1. Traer el turno abierto SIN includes: solo necesitamos la entidad
        //    rastreada para poder modificarla y guardarla. Sus totales de venta
        //    (CashSales, CardSales, etc.) ya vienen precalculados en la tabla.
        var turno = await _context.PosShifts
            .FirstOrDefaultAsync(s => s.Id == request.PosShiftId && s.Status == PosShiftStatus.Open, cancellationToken);

        if (turno == null)
        {
            throw new InvalidOperationException("El turno especificado no existe o ya se encuentra cerrado.");
        }

        // 2. Totales de movimientos menores: agregación agrupada en SQL,
        //    sin cargar las entidades PosShiftMovement completas.
        var movementTotals = await _context.PosShiftMovements
            .Where(m => m.PosShiftId == turno.Id)
            .GroupBy(m => m.Type)
            .Select(g => new { Type = g.Key, Total = g.Sum(m => m.Amount) })
            .ToListAsync(cancellationToken);

        decimal totalIncomes = movementTotals.FirstOrDefault(x => x.Type == PosShiftMovementType.Income)?.Total ?? 0m;
        decimal totalExpenses = movementTotals.FirstOrDefault(x => x.Type == PosShiftMovementType.Expense)?.Total ?? 0m;

        // 3. Datos de solo lectura para el DTO (terminal y cajero):
        //    proyecciones puntuales, sin traer las entidades completas.
        var terminalInfo = await _context.PosTerminals
            .Where(t => t.Id == turno.PosTerminalId)
            .Select(t => new { t.Name, t.Code })
            .FirstOrDefaultAsync(cancellationToken);

        var cajeroNombre = await _context.Usuarios
            .Where(u => u.Id == turno.CajeroId)
            .Select(u => u.PrimerNombre + u.PrimerAPellido)
            .FirstOrDefaultAsync(cancellationToken);

        // 4. Ventas del turno: proyección directa a DTO. El conteo de líneas
        //    se resuelve como COUNT correlacionado, sin traer SaleLineItem.
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

        // 5. Aplicar cálculos financieros al objeto en memoria (turno sigue rastreado)
        turno.TotalExpectedCash = turno.InitialCash + turno.CashSales + totalIncomes - totalExpenses;
        turno.ActualCash = request.ActualCash;
        turno.Difference = request.ActualCash - turno.TotalExpectedCash;

        // 6. Actualizar estado y fecha
        turno.Status = PosShiftStatus.Closed;
        turno.ClosedAt = DateTime.UtcNow;
        turno.Notes = request.Notes;

        // 7. Guardar cambios (turno ya está siendo rastreado por el contexto)
        await _context.SaveChangesAsync(cancellationToken);

        // 8. Ensamblar el DTO final combinando lo calculado + lo proyectado
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