namespace ERP.DATA.Services.Inventario.ProductoService;

using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class ProductoService
{
     public Task<Producto> UpdateProducto(int id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    public async Task<Producto> UpdateProducto(int id, Producto updatedProducto, CancellationToken cancellationToken = default)
    {
        // Buscar producto existente
        var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (producto == null)
            return null;
        
        producto.Nombre = updatedProducto.Nombre;
        producto.Descripcion = updatedProducto.Descripcion;
        producto.Costo_Unitario = updatedProducto.Costo_Unitario;
        producto.Precio_Venta = updatedProducto.Precio_Venta;
        producto.PorcentajeIVA = updatedProducto.PorcentajeIVA;
        producto.PorcentajeICA = updatedProducto.PorcentajeICA;
        producto.ImpuestoEspecifico = updatedProducto.ImpuestoEspecifico;
        producto.ArancelImportacion = updatedProducto.ArancelImportacion;
        producto.ExentoIVA = updatedProducto.ExentoIVA;
        producto.GravadoICA = updatedProducto.GravadoICA;
        producto.CodigoTributario = updatedProducto.CodigoTributario;
        producto.CategoriaId = updatedProducto.CategoriaId;
        producto.ProveedorId = updatedProducto.ProveedorId;
        producto.Unidad_Medida = updatedProducto.Unidad_Medida;
        producto.Peso = updatedProducto.Peso;
        producto.Volumen = updatedProducto.Volumen;
        producto.Dimensiones = updatedProducto.Dimensiones;
        producto.Imagen_Url = updatedProducto.Imagen_Url;
        producto.Notas = updatedProducto.Notas;
        producto.Tags = updatedProducto.Tags;
        producto.IsActive = updatedProducto.IsActive;

        producto.UpdatedAt = DateTime.UtcNow;
        producto.UpdatedBy = "01";

        _context.Productos.Update(producto);
        await _context.SaveChangesAsync(cancellationToken);

        return producto;
    }
}
