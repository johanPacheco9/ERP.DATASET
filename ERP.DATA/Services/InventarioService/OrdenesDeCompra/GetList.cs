using ERP.TRAN.CrossLayers.API.Inventario.OrdenDeCompra.Responses;
using ERP.TRAN.CrossLayers.API.Pos.Compras.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.OrdenesDeCompra;

public partial class OrdenesDeCompraManager
{
    /// <summary>
    /// Lista todas las órdenes de compra con filtros opcionales.
    /// </summary>
    public async Task<List<OrdenCompraSummaryDto>> GetList(
        OrdenCompraStatus? status = null,
        int? proveedorId = null,
        DateTime? desde = null,
        DateTime? hasta = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.OrdenesDeCompra
            .Include(o => o.Proveedor)
            .Include(o => o.Detalles)
            .AsNoTracking()
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        if (proveedorId.HasValue)
            query = query.Where(o => o.ProveedorId == proveedorId.Value);

        if (desde.HasValue)
            query = query.Where(o => o.Fecha >= desde.Value);

        if (hasta.HasValue)
            query = query.Where(o => o.Fecha <= hasta.Value);

        var items = await query
            .OrderByDescending(o => o.Fecha)
            .ToListAsync(cancellationToken);

        var ids = items.Select(o => o.Id).ToList();

        var recepciones = await _context.RecepcionesDeCompra
            .Where(r => ids.Contains(r.OrdenCompraId))
            .Select(r => r.OrdenCompraId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var calidades = await _context.QualityReviews
            .Where(q => ids.Contains(q.OrdenCompraId))
            .Select(q => q.OrdenCompraId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return items.Select(o => new OrdenCompraSummaryDto(
            o.Id,
            o.Proveedor?.Name ?? $"Proveedor #{o.ProveedorId}",
            o.Status.ToString(),
            GetStatusDisplay(o.Status),
            o.Fecha,
            o.Total,
            o.Detalles?.Count ?? 0,
            recepciones.Contains(o.Id),
            calidades.Contains(o.Id)
        )).ToList();
    }

    /// <summary>
    /// Obtiene el detalle completo de una OC por Id.
    /// </summary>
    public async Task<OrdenCompraDetailDto?> GetById(int id, CancellationToken cancellationToken = default)
    {
        var oc = await _context.OrdenesDeCompra
            .Include(o => o.Proveedor)
            .Include(o => o.Detalles)
                .ThenInclude(d => d.ProductoVariante)
                    .ThenInclude(v => v.ProductoBase)
            .Include(o => o.Observaciones)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        if (oc == null) return null;

        var recepcionId = await _context.RecepcionesDeCompra
            .Where(r => r.OrdenCompraId == id)
            .Select(r => (int?)r.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var qualityId = await _context.QualityReviews
            .Where(q => q.OrdenCompraId == id)
            .Select(q => (int?)q.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return new OrdenCompraDetailDto(
            oc.Id,
            oc.ProveedorId,
            oc.Proveedor?.Name ?? $"Proveedor #{oc.ProveedorId}",
            oc.Status.ToString(),
            GetStatusDisplay(oc.Status),
            oc.Fecha,
            oc.Subtotal,
            oc.Impuestos,
            oc.Total,
            recepcionId,
            qualityId,
            oc.Detalles?.Select(d => new DetalleOrdenCompraDto(
                d.Id,
                d.ProductoVarianteId,
                d.ProductoVariante?.ProductoBase?.Name,
                d.ProductoVariante?.SKU,
                d.Cantidad,
                d.CostoUnitario,
                d.Descuento,
                d.Impuesto,
                d.Total
            )).ToList() ?? new(),
            oc.Observaciones?.OrderByDescending(o => o.Fecha).Select(c => new OrdenCompraComentarioDto(
                c.Id,
                c.Texto,
                c.Fecha,
                c.EstadoAsociado.ToString()
            )).ToList() ?? new()
        );
    }

    private static string GetStatusDisplay(OrdenCompraStatus s) => s switch
    {
        OrdenCompraStatus.Draft => "Borrador",
        OrdenCompraStatus.PendingApproval => "Pendiente de Aprobación",
        OrdenCompraStatus.Approved => "Aprobada",
        OrdenCompraStatus.Sent => "Enviada al Proveedor",
        OrdenCompraStatus.PartiallyReceived => "Recibida Parcialmente",
        OrdenCompraStatus.Received => "Recibida",
        OrdenCompraStatus.Cancelled => "Cancelada",
        OrdenCompraStatus.Finalized => "Finalizada",
        _ => s.ToString()
    };
}
