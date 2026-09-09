using ERP.TRAN.CrossLayers.API.Pos.Shifts.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Shifts.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.CajaService;

public partial class CajaManager
{
    public async Task<int> OpenShiftAsync(OpenShiftRequest request, int userId, CancellationToken cancellationToken = default)
    {
        // 1. Validar que el usuario (cajero) no tenga ya un turno abierto en ninguna terminal
        var existingUserShift = await _context.PosShifts
            .Include(s => s.PosTerminal)
            .FirstOrDefaultAsync(s => s.CajeroId == userId && s.Status == PosShiftStatus.Open, cancellationToken);

        if (existingUserShift != null)
        {
            throw new InvalidOperationException($"El usuario ya tiene un turno abierto activo (Turno #{existingUserShift.Id} en '{existingUserShift.PosTerminal?.Name}'). Debe cerrarlo antes de abrir una nueva caja.");
        }

        // 2. Validar que la terminal física exista y esté activa
        var terminal = await _context.PosTerminals
            .FirstOrDefaultAsync(t => t.Id == request.PosTerminalId, cancellationToken);

        if (terminal == null)
        {
            throw new InvalidOperationException("La terminal de caja seleccionada no existe en el sistema.");
        }

        if (!terminal.IsActive)
        {
            throw new InvalidOperationException($"La terminal '{terminal.Name}' se encuentra inactiva y no puede operar.");
        }

        // 3. Validar que la terminal física no tenga un turno abierto actualmente por otro cajero
        var activeTerminalShift = await _context.PosShifts
            .Include(s => s.Cajero)
            .FirstOrDefaultAsync(s => s.PosTerminalId == request.PosTerminalId && s.Status == PosShiftStatus.Open, cancellationToken);

        if (activeTerminalShift != null)
        {
            var cajeroNombre = $"{activeTerminalShift.Cajero?.PrimerNombre} {activeTerminalShift.Cajero?.PrimerAPellido}".Trim();
            throw new InvalidOperationException($"La terminal '{terminal.Name}' ya se encuentra abierta por el cajero {cajeroNombre} (Turno #{activeTerminalShift.Id}).");
        }

        // 4. Crear la nueva entidad de turno asociada al cajero real
        var nuevoTurno = new PosShift
        {
            PosTerminalId = request.PosTerminalId,
            CajeroId = userId,
            OpenedAt = DateTime.UtcNow,
            InitialCash = request.InitialCash,
            Status = PosShiftStatus.Open,
            Notes = request.Notes,
            CashSales = 0,
            CardSales = 0,
            TransferSales = 0,
            CreditSales = 0,
            CashWithdrawals = 0,
            CashAdditions = 0,
            TotalExpectedCash = request.InitialCash,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
        };

        _context.PosShifts.Add(nuevoTurno);
        await _context.SaveChangesAsync(cancellationToken);

        return nuevoTurno.Id;
    }
}