using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Request;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.ProductoVarianteService;

public partial class ProductoVarianteService
{
    public async Task<int> AddProductoVariante(CreateProductoVarianteRequest request, CancellationToken cancellationToken)
    {
        // Verificar que el producto existe
        var producto = await _context.Productos
            .FirstOrDefaultAsync(p => p.Id == request.ProductoId, cancellationToken);

        if (producto == null)
            throw new InvalidOperationException($"No existe un producto con ID {request.ProductoId}");

        var codigoVariante = request.CodigoVariante.Trim().ToUpper();
        var existeVariante = await _context.ProductoVariantes
            .AnyAsync(v => v.CodigoVariante == codigoVariante, cancellationToken);

        if (existeVariante)
            throw new InvalidOperationException($"Ya existe una variante con el código '{codigoVariante}'");

        var variante = new ProductoVariante
        {
            ProductoId = request.ProductoId,
            CodigoVariante = codigoVariante,
            Atributos = request.Atributos,
            Codigo_Barras = request.CodigoBarras,
            Precio_Venta = request.PrecioVenta,
            Costo_Unitario = request.CostoUnitario,
            Lote = request.Lote,
            Fecha_Vencimiento = request.FechaVencimiento,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "SYSTEM",
            IsActive = true
        };

        _context.ProductoVariantes.Add(variante);
        await _context.SaveChangesAsync(cancellationToken);

        return variante.Id;
    }
}
