using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Request;
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

        var productoBaseId = requests.First().ProductoId;

        // 1. Validar que todas las variantes pertenezcan al mismo ProductoBase
        if (requests.Any(r => r.ProductoId != productoBaseId))
            throw new InvalidOperationException("Todas las variantes deben ser del mismo producto base");

        // 2. Verificar existencia del ProductoBase
        var productoBaseExiste = await _context.ProductoBase
            .AnyAsync(p => p.Id == productoBaseId, cancellationToken);

        if (!productoBaseExiste)
            throw new InvalidOperationException($"No existe un producto base con ID {productoBaseId}");

        // 3. Validar códigos únicos dentro del request
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

        // 4. Verificar que el SKU no exista ya en la base de datos
        var existentes = await _context.ProductoVariantes
            .Where(v => codigosRequest.Contains(v.SKU))
            .Select(v => v.SKU)
            .ToListAsync(cancellationToken);

        if (existentes.Any())
            throw new InvalidOperationException(
                $"Ya existen variantes con los códigos/SKUs: {string.Join(", ", existentes)}");

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var variantesEntidades = requests.Select(request => new ProductoVariante
            {
                ProductoBaseId = request.ProductoId,
                SKU = request.CodigoVariante.Trim().ToUpper(),
                CodigoBarras = request.CodigoBarras,
                Atributos = request.Atributos,
                PrecioVenta = request.PrecioVenta,
                CostoUnitario = request.CostoUnitario,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "SYSTEM",
                IsActive = true
            }).ToList();

            // 5. Inserción masiva de variantes
            _context.ProductoVariantes.AddRange(variantesEntidades);
            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            // Retornar los IDs generados automáticamente por EF Core
            return variantesEntidades.Select(v => v.Id).ToList();
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}