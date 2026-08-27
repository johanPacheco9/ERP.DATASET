using ERP.TRAN.CrossLayers.API.Pos.Shifts.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Shifts.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.CajaService;

public partial class CajaManager
{
    public async Task<int> OpenShiftAsync(OpenShiftRequest request, CancellationToken cancellationToken)
    {
        // 1. Validar que la terminal física no tenga un turno abierto actualmente
        bool hasActiveShift = await _context.PosShifts
            .AnyAsync(s => s.PosTerminalId == request.PosTerminalId && s.Status == PosShiftStatus.Open, cancellationToken);

        if (hasActiveShift)
        {
            throw new InvalidOperationException("Esta terminal de caja ya cuenta con un turno abierto. Debe cerrarlo antes de iniciar uno nuevo.");
        }

        // 2. Crear la nueva entidad de turno
        var nuevoTurno = new PosShift
        {
            PosTerminalId = request.PosTerminalId,
            CajeroId = request.CashierId,
            OpenedAt = DateTime.UtcNow,
            InitialCash = request.InitialCash,
            Status = PosShiftStatus.Open,
            CashSales = 0,
            CardSales = 0,
            TransferSales = 0,
            CreditSales = 0,
            CashWithdrawals = 0,
            CashAdditions = 0,
            TotalExpectedCash = request.InitialCash
        };

        _context.PosShifts.Add(nuevoTurno);
        await _context.SaveChangesAsync(cancellationToken);

        return nuevoTurno.Id; // Retorna el ID del turno abierto
    }
}