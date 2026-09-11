using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Pos.Terminals.Requests;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.CajaService;

public partial class CajaManager
{
    public async Task<Result<int>> Update(UpdateCajaRequest request, CancellationToken cancellationToken = default)
    {
        var caja = await _context.PosTerminals
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        
        if (caja == null)
        {
            return Result<int>.Failure(new Error("NotFound", "No existe la caja solicitada.", ErrorType.NotFound));
        }
        
        var codigoExistente = await _context.PosTerminals
            .AnyAsync(s => s.Code == request.Code && s.Id != request.Id, cancellationToken);

        if (codigoExistente)
        {
            return Result<int>.Failure(new Error("Conflict", $"Ya existe otra caja registrada con el código '{request.Code}'.", ErrorType.Conflict));
        }
        
        var bodegaValida = await _context.Warehouse
            .AnyAsync(w => w.Id == request.WarehouseId && w.StoreId == request.StoreId, cancellationToken);

        if (!bodegaValida)
        {
            return Result<int>.Failure(new Error("BadRequest", "La bodega seleccionada no pertenece a la tienda especificada.", ErrorType.Validation));
        }

        var userId = await userManager.GetUserId();
        
        caja.Name = request.Name;
        caja.Code = request.Code;
        caja.StoreId = request.StoreId;
        caja.WarehouseId = request.WarehouseId;
        caja.Prefix = request.Prefix;
        caja.CurrentConsecutive = request.CurrentConsecutive;
        caja.DianResolutionNumber = request.DianResolutionNumber;
        caja.DianResolutionDate = request.DianResolutionDate;
        caja.FromNumber = request.FromNumber;
        caja.ToNumber = request.ToNumber;
        caja.IsActive = request.IsActive;
        caja.UpdatedAt = DateTime.UtcNow;
        caja.UpdatedBy = userId.Value;
        
        _context.PosTerminals.Update(caja);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(caja.Id);
    }
}