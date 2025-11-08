namespace ERP.DATA.Services.Inventario.ProductoService;

using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

public partial class ProductoService
{
    public async Task<Producto> AddProductoAsync(Producto producto, CancellationToken cancellationToken = default)
    {
        // Generar un código único basado en los primeros 3 caracteres del nombre o código  o no sé como xd
        var codigo = $"PRD-{producto.Codigo[..3].ToUpper()}";

        var exists = await _context.Productos.AnyAsync(c => c.Codigo == codigo, cancellationToken);
        if (exists)
            throw new InvalidOperationException($"Ya existe un producto con el código '{codigo}'.");

        var nuevoProducto = new Producto
        {
            Id = Guid.NewGuid(),
            Codigo = codigo,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Costo_Unitario = producto.Costo_Unitario,
            Precio_Venta = producto.Precio_Venta,
            PorcentajeIVA = producto.PorcentajeIVA,
            PorcentajeICA = producto.PorcentajeICA,
            ImpuestoEspecifico = producto.ImpuestoEspecifico,
            ArancelImportacion = producto.ArancelImportacion,
            ExentoIVA = producto.ExentoIVA,
            GravadoICA = producto.GravadoICA,
            CodigoTributario = producto.CodigoTributario,
            CategoriaId = producto.CategoriaId,
            ProveedorId = producto.ProveedorId,
            Unidad_Medida = producto.Unidad_Medida,
            Peso = producto.Peso,
            Volumen = producto.Volumen,
            Dimensiones = producto.Dimensiones,
            Imagen_Url = producto.Imagen_Url,
            Notas = producto.Notas,
            Tags = producto.Tags,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "01",
            IsActive = true
        };

        _context.Productos.Add(nuevoProducto);
        await _context.SaveChangesAsync(cancellationToken);

        return nuevoProducto;
    }
}
