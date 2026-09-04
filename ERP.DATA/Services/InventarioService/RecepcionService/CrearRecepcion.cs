using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Inventario.Recepcion.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Recepcion.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Compras.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Compras;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Compras.Recepciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.RecepcionService;

public partial class RecepcionCompraManager
{
    /// <summary>
    /// Registra la recepción física de una OC (aprobada o enviada).
    /// No afecta stock: las unidades aprobadas ingresan al inventario tras el control de calidad.
    /// </summary>
    public async Task<Result<RecepcionDetailDto>> Crear(
        CreateRecepcionRequest request,
        int creadoPor,
        CancellationToken cancellationToken = default)
    {
        var oc = await _context.OrdenesDeCompra
            .Include(o => o.Detalles)
                .ThenInclude(d => d.ProductoVariante)
                    .ThenInclude(v => v.ProductoBase)
            .Include(o => o.Observaciones)
            .FirstOrDefaultAsync(o => o.Id == request.OrdenCompraId, cancellationToken);

        if (oc == null)
            return Result<RecepcionDetailDto>.Failure(Error.NotFound("Recepcion.OCNotFound",
                $"La orden de compra #{request.OrdenCompraId} no existe."));

        if (oc.Status != OrdenCompraStatus.Approved && oc.Status != OrdenCompraStatus.Sent)
            return Result<RecepcionDetailDto>.Failure(Error.Failure("Recepcion.InvalidStatus",
                "Solo se puede recibir mercancía de órdenes Aprobadas o Enviadas al proveedor."));

        var yaRecibida = await _context.RecepcionesDeCompra
            .AnyAsync(r => r.OrdenCompraId == request.OrdenCompraId, cancellationToken);

        if (yaRecibida)
            return Result<RecepcionDetailDto>.Failure(Error.Failure("Recepcion.AlreadyReceived",
                "Esta orden ya tiene una recepción registrada."));

        var bodega = await _context.Warehouse
            .FirstOrDefaultAsync(w => w.Id == request.BodegaId, cancellationToken);

        if (bodega == null)
            return Result<RecepcionDetailDto>.Failure(Error.NotFound("Recepcion.BodegaNotFound",
                $"La bodega #{request.BodegaId} no existe."));

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // 1. Crear cabecera de recepción
            var recepcion = new RecepcionCompra
            {
                OrdenCompraId = request.OrdenCompraId,
                BodegaId = request.BodegaId,
                FechaRecepcion = DateTime.UtcNow,
                Status = RecepcionCompraStatus.EnControlCalidad,
                GuiaRemisionProveedor = request.GuiaRemisionProveedor,
                Observaciones = request.Observaciones,
                CreatedBy = 1,
                CreatedAt = DateTime.UtcNow
            };

            // 2. Procesar cada línea de la recepción usando el servicio de movimientos existente
            foreach (var detalle in request.Detalles)
            {
                if (detalle.CantidadRecibida <= 0) continue;

                var detalleOC = oc.Detalles.FirstOrDefault(d => d.Id == detalle.DetalleOrdenCompraId);
                if (detalleOC == null) continue;

                recepcion.Detalles.Add(new DetalleRecepcionCompra
                {
                    DetalleOrdenCompraId = detalle.DetalleOrdenCompraId,
                    ProductoVarianteId = detalle.ProductoVarianteId,
                    CantidadEsperada = detalle.CantidadEsperada,
                    CantidadRecibida = detalle.CantidadRecibida,
                    ObservacionItem = detalle.ObservacionItem
                });

                // La recepción confirma custodia física; el stock se ingresa al aprobar calidad.
            }

            _context.RecepcionesDeCompra.Add(recepcion);

            // 3. Actualizar estado de la OC → Received (pendiente de control de calidad)
            oc.Status = OrdenCompraStatus.Received;
            oc.Observaciones.Add(new OrdenCompraObservaciones
            {
                Texto = $"Mercancía recibida físicamente en bodega '{bodega.Name}'. " +
                        $"Guía: {request.GuiaRemisionProveedor ?? "N/A"}. " +
                        $"Total unidades: {request.Detalles.Sum(d => d.CantidadRecibida)}.",
                Fecha = DateTime.UtcNow,
                EstadoAsociado = OrdenCompraStatus.Received,
                UsuarioId = creadoPor
            });

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<RecepcionDetailDto>.Success(new RecepcionDetailDto(
                recepcion.Id,
                recepcion.OrdenCompraId,
                recepcion.BodegaId,
                bodega.Name,
                recepcion.Status.ToString(),
                "En Control de Calidad",
                recepcion.FechaRecepcion,
                recepcion.GuiaRemisionProveedor,
                recepcion.Observaciones,
                recepcion.Detalles.Select(d => new DetalleRecepcionDto(
                    d.Id,
                    d.ProductoVarianteId,
                    null,
                    null,
                    d.CantidadEsperada,
                    d.CantidadRecibida,
                    d.ObservacionItem
                )).ToList()
            ));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error al registrar recepción de OC #{Id}", request.OrdenCompraId);
            return Result<RecepcionDetailDto>.Failure(Error.Failure("Recepcion.Error", ex.Message));
        }
    }
}
