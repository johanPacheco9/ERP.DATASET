using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Inventario.OrdenDeCompra.Responses;
using ERP.TRAN.CrossLayers.API.Pos.Compras.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Compras;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.OrdenesDeCompra;

public partial class OrdenesDeCompraManager
{
    /// <summary>
    /// Aprueba una OC en estado PendingApproval → Approved.
    /// </summary>
    public async Task<Result<OrdenCompraDetailDto>> Aprobar(
        int ordenCompraId,
        int aprobadoPor,
        string? observacion = null,
        CancellationToken cancellationToken = default)
    {
        var oc = await _context.OrdenesDeCompra
            .Include(o => o.Observaciones)
            .FirstOrDefaultAsync(o => o.Id == ordenCompraId, cancellationToken);

        if (oc == null)
            return Result<OrdenCompraDetailDto>.Failure(Error.NotFound("OC.NotFound", $"La orden {ordenCompraId} no existe."));

        if (oc.Status != OrdenCompraStatus.PendingApproval)
            return Result<OrdenCompraDetailDto>.Failure(Error.Failure("OC.InvalidStatus",
                $"Solo se pueden aprobar órdenes en estado 'Pendiente de Aprobación'. Estado actual: {GetStatusDisplay(oc.Status)}"));

        oc.Status = OrdenCompraStatus.Approved;

        oc.Observaciones.Add(new OrdenCompraObservaciones
        {
            Texto = observacion ?? $"Orden aprobada por {aprobadoPor}.",
            Fecha = DateTime.UtcNow,
            EstadoAsociado = OrdenCompraStatus.Approved,
            UsuarioId = aprobadoPor
        });

        await _context.SaveChangesAsync(cancellationToken);

        var dto = await GetById(ordenCompraId, cancellationToken);
        return Result<OrdenCompraDetailDto>.Success(dto!);
    }

    /// <summary>
    /// Cambia estado Approved → Sent (enviada al proveedor).
    /// </summary>
    public async Task<Result<OrdenCompraDetailDto>> Enviar(
        int ordenCompraId,
        int enviadoPor,
        string? observacion = null,
        CancellationToken cancellationToken = default)
    {
        var oc = await _context.OrdenesDeCompra
            .Include(o => o.Observaciones)
            .FirstOrDefaultAsync(o => o.Id == ordenCompraId, cancellationToken);

        if (oc == null)
            return Result<OrdenCompraDetailDto>.Failure(Error.NotFound("OC.NotFound", $"La orden {ordenCompraId} no existe."));

        if (oc.Status != OrdenCompraStatus.Approved)
            return Result<OrdenCompraDetailDto>.Failure(Error.Failure("OC.InvalidStatus",
                $"Solo se pueden enviar órdenes en estado 'Aprobada'. Estado actual: {GetStatusDisplay(oc.Status)}"));

        oc.Status = OrdenCompraStatus.Sent;

        oc.Observaciones.Add(new OrdenCompraObservaciones
        {
            Texto = observacion ?? $"Orden enviada al proveedor por {enviadoPor}.",
            Fecha = DateTime.UtcNow,
            EstadoAsociado = OrdenCompraStatus.Sent,
            UsuarioId = enviadoPor
        });

        await _context.SaveChangesAsync(cancellationToken);

        var dto = await GetById(ordenCompraId, cancellationToken);
        return Result<OrdenCompraDetailDto>.Success(dto!);
    }

    /// <summary>
    /// Cancela una OC. Solo si no tiene recepción registrada.
    /// </summary>
    public async Task<Result> Cancelar(
        int ordenCompraId,
        int canceladoPor,
        string motivo,
        CancellationToken cancellationToken = default)
    {
        var oc = await _context.OrdenesDeCompra
            .Include(o => o.Observaciones)
            .FirstOrDefaultAsync(o => o.Id == ordenCompraId, cancellationToken);

        if (oc == null)
            return Result.Failure(Error.NotFound("OC.NotFound", $"La orden {ordenCompraId} no existe."));

        if (oc.Status == OrdenCompraStatus.Received || oc.Status == OrdenCompraStatus.Finalized)
            return Result.Failure(Error.Failure("OC.CannotCancel",
                "No se puede cancelar una orden que ya fue recibida o finalizada."));

        if (oc.Status == OrdenCompraStatus.Cancelled)
            return Result.Failure(Error.Failure("OC.AlreadyCancelled", "La orden ya está cancelada."));

        var tieneRecepcion = await _context.RecepcionesDeCompra
            .AnyAsync(r => r.OrdenCompraId == ordenCompraId, cancellationToken);

        if (tieneRecepcion)
            return Result.Failure(Error.Failure("OC.HasReception",
                "No se puede cancelar porque ya tiene una recepción de mercancía registrada."));

        oc.Status = OrdenCompraStatus.Cancelled;

        oc.Observaciones.Add(new OrdenCompraObservaciones
        {
            Texto = $"Cancelada por {canceladoPor}. Motivo: {motivo}",
            Fecha = DateTime.UtcNow,
            EstadoAsociado = OrdenCompraStatus.Cancelled,
            UsuarioId = canceladoPor
        });

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
