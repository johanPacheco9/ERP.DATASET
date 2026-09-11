using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Pos.Terminals.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.CajaService;

public partial class CajaManager
{
    public async Task<Result<PosTerminalDto>> GetById(int cajaId, CancellationToken cancellationToken = default)
    {
        var caja = await _context.PosTerminals
            .AsNoTracking()
            .Include(c => c.Store)
            .Include(c => c.Warehouse)
            // Incluye turnos activos si tu modelo los relaciona directamente
            // .Include(c => c.Shifts) 
            .FirstOrDefaultAsync(s => s.Id == cajaId, cancellationToken);

        if (caja == null)
        {
            return Result<PosTerminalDto>.Failure(new Error("NotFound", "No existe la caja solicitada", ErrorType.NotFound));
        }
        var dto = new PosTerminalDto(
            caja.Id,
            caja.Name,
            caja.Code,
            caja.StoreId,
            caja.Store?.Name ?? string.Empty,
            caja.WarehouseId,
            caja.Warehouse?.Name ?? string.Empty,
            caja.Prefix,
            caja.CurrentConsecutive,
            caja.DianResolutionNumber,
            caja.IsActive,
            false, // Ajustar según lógica de turno activo
            null,  // activeShift?.Id
            null   // activeShift?.CashierName
        );

        return Result<PosTerminalDto>.Success(dto);
    }
}