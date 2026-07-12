using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.ProductoVarianteService;

public partial class ProductVariantService
{
    public async Task<List<int>> AddProductoVariantes(
       List<CreateProductoVarianteRequest> requests,
       CancellationToken cancellationToken = default)
    {
        if (requests == null || !requests.Any())
            throw new ArgumentException("Debe proporcionar al menos una variante");

        var productoId = requests.First().ProductoId;

        // Validar que todas sean del mismo producto
        if (requests.Any(r => r.ProductoId != productoId))
            throw new InvalidOperationException("Todas las variantes deben ser del mismo producto");

        // Verificar que el producto existe
        var producto = await _context.LineaProductos
            .FirstOrDefaultAsync(p => p.Id == productoId, cancellationToken);

        if (producto == null)
            throw new InvalidOperationException($"No existe un producto con ID {productoId}");

        // Validar códigos únicos en el request
        var codigosRequest = requests
            .Select(r => r.CodigoVariante.Trim().ToUpper())
            .ToList();

        var duplicadosEnRequest = codigosRequest
            .GroupBy(c => c)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicadosEnRequest.Any())
            throw new InvalidOperationException(
                $"Códigos duplicados en el request: {string.Join(", ", duplicadosEnRequest)}");

        // Verificar que no existan en la BD
        var existentes = await _context.Productos
            .Where(v => codigosRequest.Contains(v.SKU))
            .Select(v => v.SKU)
            .ToListAsync(cancellationToken);

        if (existentes.Any())
            throw new InvalidOperationException(
                $"Ya existen variantes con los códigos: {string.Join(", ", existentes)}");

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var variantesCreadas = new List<int>();

            foreach (var request in requests)
            {
                var variante = new Producto
                {
                    LineaProductoId = request.ProductoId,
                    SKU = request.CodigoVariante.Trim().ToUpper(),
                    Atributos = request.Atributos,
                    CodigoBarras = request.CodigoBarras,
                    PrecioVenta = request.PrecioVenta,
                    CostoUnitario = request.CostoUnitario,
                    Lote = request.Lote,
                    FechaVencimiento = request.FechaVencimiento,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "SYSTEM",
                    IsActive = true,
                    BodegaId = request.BodegaId ?? 1,
                    Status = ProductoStatus.Available
                };

                _context.Productos.Add(variante);
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Obtener los IDs generados
            variantesCreadas = await _context.Productos
                .Where(v => codigosRequest.Contains(v.SKU))
                .Select(v => v.Id)
                .ToListAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return variantesCreadas;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
