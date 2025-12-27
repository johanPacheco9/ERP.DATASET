namespace ERP.DATA.Services.Inventario.ProductoService;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

public partial class ProductoService
{
    public async Task<int> AddProductoAsync(
        CreateProductoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Codigo) || request.Codigo.Length < 3)
            throw new ArgumentException("El código debe tener al menos 3 caracteres.");

        var codigoProducto = request.Codigo.Trim().ToUpper();

        var exists = await _context.Productos
            .AnyAsync(p => p.Codigo == codigoProducto, cancellationToken);

        if (exists)
            throw new InvalidOperationException($"Ya existe un producto con el código '{codigoProducto}'.");

        var producto = new Producto
        {
            Codigo = codigoProducto,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            CategoriaId = request.CategoriaId,
            ProveedorId = request.ProveedorId,
            Unidad_Medida = request.Unidad_Medida ?? "UND",
            Peso = request.Peso,
            Volumen = request.Volumen,
            Dimensiones = request.Dimensiones,
            Imagen_Url = request.Imagen_Url,
            Notas = request.Notas,
            Tags = request.Tags,
            Costo_Unitario = request.Costo_Unitario,
            Precio_Venta = request.Precio_Venta,
            PorcentajeIVA = request.PorcentajeIVA,
            PorcentajeICA = request.PorcentajeICA,
            ImpuestoEspecifico = request.ImpuestoEspecifico,
            ArancelImportacion = request.ArancelImportacion,
            ExentoIVA = request.ExentoIVA,
            GravadoICA = request.GravadoICA,
            CodigoTributario = request.CodigoTributario,
            Es_Perecedero = request.Es_Perecedero,
            Estado = 0,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "SYSTEM",
            IsActive = true
        };

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync(cancellationToken);

        return producto.Id;
    }
}
