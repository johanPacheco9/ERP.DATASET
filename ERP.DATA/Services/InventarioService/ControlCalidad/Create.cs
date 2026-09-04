using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Inventario.ControlCalidad.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.ControlCalidad.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.API.Pos.Compras.Responses;
using ERP.TRAN.CrossLayers.API.QualityReviews.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Compras;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Compras.Recepciones;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.QualityReviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.ControlCalidad;

public partial class ControlCalidadManager
{
    /// <summary>
    /// Crea el control de calidad para una recepción.
    /// </summary>
    public async Task<Result<QualityReviewDetailDto>> Create(
        CreateQualityReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        var oc = await _context.OrdenesDeCompra
            .FirstOrDefaultAsync(o => o.Id == request.OrdenCompraId, cancellationToken);

        if (oc == null)
            return Result<QualityReviewDetailDto>.Failure(Error.NotFound("QC.OCNotFound",
                $"La orden de compra #{request.OrdenCompraId} no existe."));

        var yaExiste = await _context.QualityReviews
            .AnyAsync(q => q.OrdenCompraId == request.OrdenCompraId, cancellationToken);

        if (yaExiste)
            return Result<QualityReviewDetailDto>.Failure(Error.Failure("QC.AlreadyExists",
                "Ya existe un control de calidad para esta orden de compra."));

        var qr = new QualityReview
        {
            OrdenCompraId = request.OrdenCompraId,
            Status = QualityReviewStatus.Pendiente,
            ObservacionesGenerales = request.ObservacionesGenerales,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1,
            Detalles = request.Items.Select(i => new QualityReviewDetail
            {
                DetalleOrdenCompraId = i.DetalleOrdenCompraId,
                CantidadRecibida = i.CantidadRecibida,
                CantidadAprobada = i.CantidadAprobada,
                CantidadRechazada = i.CantidadRechazada,
                MotivoRechazo = i.MotivoRechazo
            }).ToList()
        };

        _context.QualityReviews.Add(qr);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<QualityReviewDetailDto>.Success(MapToDto(qr));
    }

    /// <summary>
    /// Aprueba el control de calidad, ingresa únicamente las cantidades aprobadas y finaliza la OC.
    /// </summary>
    public async Task<Result<QualityReviewDetailDto>> Aprobar(
        AprobarQualityReviewRequest request,
        int aprobadoPor,
        CancellationToken cancellationToken = default)
    {
        var qr = await _context.QualityReviews
            .Include(q => q.Detalles)
                .ThenInclude(d => d.DetalleOrdenCompra)
            .FirstOrDefaultAsync(q => q.Id == request.QualityReviewId, cancellationToken);

        if (qr == null)
            return Result<QualityReviewDetailDto>.Failure(Error.NotFound("QC.NotFound",
                "Control de calidad no encontrado."));

        if (qr.Status != QualityReviewStatus.Pendiente)
            return Result<QualityReviewDetailDto>.Failure(Error.Failure("QC.AlreadyProcessed",
                "Este control de calidad ya fue procesado."));

        bool hayRechazados = qr.Detalles.Any(d => d.CantidadRechazada > 0);

        var recepcion = await _context.RecepcionesDeCompra
            .FirstOrDefaultAsync(r => r.OrdenCompraId == qr.OrdenCompraId, cancellationToken);
        if (recepcion == null)
            return Result<QualityReviewDetailDto>.Failure(Error.NotFound("QC.RecepcionNotFound",
                "No existe una recepción física asociada a este control de calidad."));

        var detallesAprobados = qr.Detalles.Where(d => d.CantidadAprobada > 0).ToList();
        if (detallesAprobados.Any(d => decimal.Truncate(d.CantidadAprobada) != d.CantidadAprobada))
            return Result<QualityReviewDetailDto>.Failure(Error.Validation("QC.InvalidQuantity",
                "Las cantidades aprobadas deben ser enteras para poder crear unidades de inventario."));

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            foreach (var detalle in detallesAprobados)
            {
                await _movimientosManager.RegistrarEnTransaccion(new RegistrarMovimientoEntradaRequest
                {
                    BodegaId = recepcion.BodegaId,
                    ProductoVarianteId = detalle.DetalleOrdenCompra.ProductoVarianteId,
                    Cantidad = (int)detalle.CantidadAprobada,
                    Motivo = $"Ingreso aprobado por calidad — OC #{qr.OrdenCompraId}",
                    ReferenciaId = qr.OrdenCompraId,
                    ReferenciaTipo = "orden_compra"
                }, cancellationToken);
            }

            qr.Status = hayRechazados ? QualityReviewStatus.AprobadoParcial : QualityReviewStatus.Aprobado;
            qr.ObservacionesGenerales = (qr.ObservacionesGenerales ?? "") +
                $"\nAprobado por {aprobadoPor} el {DateTime.UtcNow:dd/MM/yyyy HH:mm}.";

            // Actualizar estado de la OC → Finalized
            var oc = await _context.OrdenesDeCompra
                .Include(o => o.Observaciones)
                .FirstOrDefaultAsync(o => o.Id == qr.OrdenCompraId, cancellationToken);

            if (oc != null)
            {
                oc.Status = OrdenCompraStatus.Finalized;
                oc.Observaciones.Add(new OrdenCompraObservaciones
                {
                    Texto = hayRechazados
                        ? $"Control de calidad aprobado parcialmente. Mercancía rechazada: {qr.Detalles.Sum(d => d.CantidadRechazada)} unidades."
                        : $"Control de calidad aprobado completamente por {aprobadoPor}.",
                    Fecha = DateTime.UtcNow,
                    EstadoAsociado = OrdenCompraStatus.Finalized,
                    UsuarioId = aprobadoPor
                });
            }

            // Actualizar recepción a Finalizado
            recepcion.Status = RecepcionCompraStatus.Finalizado;

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<QualityReviewDetailDto>.Success(MapToDto(qr));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error al ingresar inventario al aprobar control de calidad #{QualityReviewId}", qr.Id);
            return Result<QualityReviewDetailDto>.Failure(Error.Failure("QC.InventoryEntryFailed",
                "No se pudo ingresar el inventario; la aprobación no fue aplicada."));
        }
    }

    /// <summary>
    /// Rechaza el control de calidad completamente.
    /// </summary>
    public async Task<Result<QualityReviewDetailDto>> Rechazar(
        RechazarQualityReviewRequest request,
        int rechazadoPor,
        CancellationToken cancellationToken = default)
    {
        var qr = await _context.QualityReviews
            .Include(q => q.Detalles)
            .FirstOrDefaultAsync(q => q.Id == request.QualityReviewId, cancellationToken);

        if (qr == null)
            return Result<QualityReviewDetailDto>.Failure(Error.NotFound("QC.NotFound",
                "Control de calidad no encontrado."));

        if (qr.Status != QualityReviewStatus.Pendiente)
            return Result<QualityReviewDetailDto>.Failure(Error.Failure("QC.AlreadyProcessed",
                "Este control de calidad ya fue procesado."));

        qr.Status = QualityReviewStatus.Rechazado;

        var oc = await _context.OrdenesDeCompra
            .Include(o => o.Observaciones)
            .FirstOrDefaultAsync(o => o.Id == qr.OrdenCompraId, cancellationToken);

        if (oc != null)
        {
            oc.Observaciones.Add(new OrdenCompraObservaciones
            {
                Texto = $"Control de calidad RECHAZADO por {rechazadoPor}. Motivo: {request.MotivoRechazo}",
                Fecha = DateTime.UtcNow,
                EstadoAsociado = oc.Status,
                UsuarioId = rechazadoPor
            });
        }

        var recepcion = await _context.RecepcionesDeCompra
            .FirstOrDefaultAsync(r => r.OrdenCompraId == qr.OrdenCompraId, cancellationToken);
        if (recepcion != null)
            recepcion.Status = RecepcionCompraStatus.RechazadoParcial;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<QualityReviewDetailDto>.Success(MapToDto(qr));
    }

    /// <summary>
    /// Obtiene el control de calidad por Id de OC.
    /// </summary>
    public async Task<QualityReviewDetailDto?> GetByOrdenCompraId(int ocId, CancellationToken cancellationToken = default)
    {
        var qr = await _context.QualityReviews
            .Include(q => q.Detalles)
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.OrdenCompraId == ocId, cancellationToken);

        return qr == null ? null : MapToDto(qr);
    }

    private static QualityReviewDetailDto MapToDto(QualityReview qr) => new(
        qr.Id,
        qr.OrdenCompraId,
        0,
        qr.Status.ToString(),
        qr.Status switch
        {
            QualityReviewStatus.Pendiente => "Pendiente",
            QualityReviewStatus.Aprobado => "Aprobado",
            QualityReviewStatus.AprobadoParcial => "Aprobado Parcialmente",
            QualityReviewStatus.Rechazado => "Rechazado",
            _ => qr.Status.ToString()
        },
        qr.Detalles?.Sum(d => d.CantidadRecibida) ?? 0,
        qr.Detalles?.Sum(d => d.CantidadAprobada) ?? 0,
        qr.Detalles?.Sum(d => d.CantidadRechazada) ?? 0,
        qr.ObservacionesGenerales,
        qr.Detalles?.Select(d => new QualityReviewItemDto(
            d.Id,
            0,
            null,
            null,
            d.CantidadRecibida,
            d.CantidadAprobada,
            d.CantidadRechazada,
            d.MotivoRechazo
        )).ToList() ?? new()
    );
}
