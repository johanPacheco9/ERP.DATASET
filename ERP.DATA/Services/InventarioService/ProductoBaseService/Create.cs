using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoBase.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.ProductoBaseService;

public partial class ProductoBaseService
{
    /// <summary>
    /// Se usa para crear el catálogo, no las unidades del producto.
    /// </summary>
   public async Task<int> AddProductoAsync(
        CreateProductoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Codigo) || request.Codigo.Length < 3)
            throw new ArgumentException("El código debe tener al menos 3 caracteres.");

        var codigoProducto = request.Codigo.Trim().ToUpper();

        var exists = await context.ProductoBase
            .AnyAsync(p => p.Code == codigoProducto, cancellationToken);

        if (exists)
            throw new InvalidOperationException($"Ya existe un producto con el código '{codigoProducto}'.");

        if (!request.BodegaId.HasValue)
        {
            // Lógica si aplica bodega por defecto o validación
        }

        var producto = new ProductoBase
        {
            Code = codigoProducto,
            Name = request.Nombre,
            Description = request.Descripcion,
            SupplierId = request.ProveedorId,
            MarcaId = request.MarcaId ?? 0, // Asignación correcta de la marca única
            UnidadMedida = request.Unidad_Medida ?? "UND",
            Peso = request.Peso,
            Volumen = request.Volumen,
            Dimensiones = request.Dimensiones,
            ImagenUrl = request.Imagen_Url,
            Notas = request.Notas,
            Tags = request.Tags,
            CostoUnitario = request.Costo_Unitario,
            PrecioVenta = request.Precio_Venta,
            PorcentajeIVA = request.PorcentajeIVA,
            PorcentajeICA = request.PorcentajeICA,
            ImpuestoEspecifico = request.ImpuestoEspecifico,
            ArancelImportacion = request.ArancelImportacion,
            ExentoIVA = request.ExentoIVA,
            GravadoICA = request.GravadoICA,
            CodigoTributario = request.CodigoTributario,
            EsPerecedero = request.EsPerecedero,
            BaseStatus = 0,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1,
            IsActive = true
        };
        
        // Mapeo correcto de la relación de muchos a muchos con categorías
        if (request.CategoriasIds != null && request.CategoriasIds.Any())
        {
            foreach (var catId in request.CategoriasIds)
            {
                producto.Categorias.Add(new ProductoBaseCategory
                {
                    CategoryId = catId
                });
            }
        }

        context.ProductoBase.Add(producto);
        await context.SaveChangesAsync(cancellationToken);

        return producto.Id;
    }
}