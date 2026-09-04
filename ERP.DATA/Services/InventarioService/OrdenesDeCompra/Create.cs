using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Inventario.OrdenDeCompra.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.OrdenDeCompra.Responses;
using ERP.TRAN.CrossLayers.API.Pos.Compras.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Compras;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.OrdenesDeCompra;

public partial class OrdenesDeCompraManager
{
    public async Task<Result<OrdenCompraDetailDto>> Create(CreateOrdenCompraRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (request.Detalles == null || !request.Detalles.Any())
                return Result<OrdenCompraDetailDto>.Failure(Error.Failure("OrdenCompra.EmptyDetails", "La orden de compra debe contener al menos un detalle."));

            var proveedor = await _context.Proveedores
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.ProveedorId, cancellationToken);

            if (proveedor == null)
                return Result<OrdenCompraDetailDto>.Failure(Error.Failure("OrdenCompra.ProveedorNotFound", "El proveedor especificado no existe."));

            var ordenCompra = new OrdenCompra
            {
                ProveedorId = request.ProveedorId,
                Fecha = DateTime.UtcNow,
                Status = OrdenCompraStatus.PendingApproval,
                Detalles = request.Detalles.Select(d => new DetalleOrdenCompra
                {
                    ProductoVarianteId = d.ProductoVarianteId,
                    Cantidad = d.Cantidad,
                    CostoUnitario = d.CostoUnitario,
                    Descuento = d.Descuento,
                    Impuesto = d.Impuesto,
                    Total = (d.Cantidad * d.CostoUnitario) - d.Descuento + d.Impuesto
                }).ToList(),
                Observaciones = request.Observaciones?
                    .Where(obs => !string.IsNullOrWhiteSpace(obs))
                    .Select(obs => new OrdenCompraObservaciones
                    {
                        Texto = obs,
                        Fecha = DateTime.UtcNow,
                        UsuarioId = 1,
                        EstadoAsociado = OrdenCompraStatus.PendingApproval
                    }).ToList() ?? new List<OrdenCompraObservaciones>()
            };

            ordenCompra.Subtotal = ordenCompra.Detalles.Sum(d => (d.Cantidad * d.CostoUnitario) - d.Descuento);
            ordenCompra.Impuestos = ordenCompra.Detalles.Sum(d => d.Impuesto);
            ordenCompra.Total = ordenCompra.Subtotal + ordenCompra.Impuestos;

            _context.OrdenesDeCompra.Add(ordenCompra);
            await _context.SaveChangesAsync(cancellationToken);

            var dto = new OrdenCompraDetailDto(
                ordenCompra.Id,
                proveedor.Id,
                proveedor.Name, // Usamos la variable proveedor en lugar de ordenCompra.Proveedor
                ordenCompra.Status.ToString(), // O Status.GetDisplayName() si prefieres
                ordenCompra.Status.GetDisplayName(),
                ordenCompra.Fecha,
                ordenCompra.Subtotal,
                ordenCompra.Impuestos,
                ordenCompra.Total,
                null, // RecepcionId (al crearse, aún no tiene recepción)
                null, // QualityReviewId (al crearse, aún no tiene control de calidad)
                ordenCompra.Detalles.Select(d => new DetalleOrdenCompraDto(
                    d.Id,
                    d.ProductoVarianteId,
                    d.ProductoVariante?.ProductoBase?.Name, // Asegúrate de incluir .Include si necesitas esto, o déjalo null/vacío al crear
                    d.ProductoVariante?.SKU,
                    d.Cantidad,
                    d.CostoUnitario,
                    d.Descuento,
                    d.Impuesto,
                    d.Total
                )).ToList(),
                ordenCompra.Observaciones.Select(c => new OrdenCompraComentarioDto(
                    c.Id,
                    c.Texto,
                    c.Fecha,
                    c.EstadoAsociado.ToString()
                )).ToList()
            );

            return Result<OrdenCompraDetailDto>.Success(dto);
        }
        catch (Exception e)
        {
            return Result<OrdenCompraDetailDto>.Failure(
                Error.Failure("OrdenCompra.CreationError", e.Message)
            );
        }
    }
}
