using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Request;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.ProductoVarianteService;

public partial class ProductoVarianteService
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
        var producto = await _context.Productos
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
        var existentes = await _context.ProductoVariantes
            .Where(v => codigosRequest.Contains(v.CodigoVariante))
            .Select(v => v.CodigoVariante)
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
                var variante = new ProductoVariante
                {
                    ProductoId = request.ProductoId,
                    CodigoVariante = request.CodigoVariante.Trim().ToUpper(),
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
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Obtener los IDs generados
            variantesCreadas = await _context.ProductoVariantes
                .Where(v => codigosRequest.Contains(v.CodigoVariante))
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
