using ERP.TRAN.CrossLayers.API.Inventario.Recepcion.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Compras.Recepciones;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.RecepcionService;

public partial class RecepcionCompraManager
{
    /// <summary>
    /// Obtiene el detalle de una recepción por Id.
    /// </summary>
    public async Task<RecepcionDetailDto?> GetById(int id, CancellationToken cancellationToken = default)
    {
        var r = await _context.RecepcionesDeCompra
            .Include(r => r.Detalles)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (r == null) return null;

        var bodega = await _context.Warehouse.AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == r.BodegaId, cancellationToken);

        var statusDisplay = r.Status switch
        {
            RecepcionCompraStatus.Borrador => "Borrador",
            RecepcionCompraStatus.RecibidoFisico => "Recibido Físicamente",
            RecepcionCompraStatus.EnControlCalidad => "En Control de Calidad",
            RecepcionCompraStatus.Finalizado => "Finalizado",
            RecepcionCompraStatus.RechazadoParcial => "Rechazado Parcialmente",
            _ => r.Status.ToString()
        };

        return new RecepcionDetailDto(
            r.Id,
            r.OrdenCompraId,
            r.BodegaId,
            bodega?.Name ?? $"Bodega #{r.BodegaId}",
            r.Status.ToString(),
            statusDisplay,
            r.FechaRecepcion,
            r.GuiaRemisionProveedor,
            r.Observaciones,
            r.Detalles.Select(d => new DetalleRecepcionDto(
                d.Id,
                d.DetalleOrdenCompraId,
                "Producto Recibido",
                null,
                d.CantidadEsperada,
                d.CantidadRecibida,
                d.ObservacionItem
            )).ToList()
        );
    }
}