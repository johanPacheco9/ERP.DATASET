using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using Microsoft.EntityFrameworkCore;
namespace ERP.DATA.Services.InventarioService.ProductService;

public partial class ProductService
{
     public Task<LineaProducto> UpdateProducto(int id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    public async Task<LineaProducto> UpdateProducto(int id, LineaProducto updatedProducto, CancellationToken cancellationToken = default)
    {
        var producto = await context.LineaProductos.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (producto == null)
            return null;
        
        producto.Name = updatedProducto.Name;
        producto.Description = updatedProducto.Description;
        producto.CostoUnitario = updatedProducto.CostoUnitario;
        producto.PrecioVenta = updatedProducto.PrecioVenta;
        producto.PorcentajeIVA = updatedProducto.PorcentajeIVA;
        producto.PorcentajeICA = updatedProducto.PorcentajeICA;
        producto.ImpuestoEspecifico = updatedProducto.ImpuestoEspecifico;
        producto.ArancelImportacion = updatedProducto.ArancelImportacion;
        producto.ExentoIVA = updatedProducto.ExentoIVA;
        producto.GravadoICA = updatedProducto.GravadoICA;
        producto.CodigoTributario = updatedProducto.CodigoTributario;
        producto.CategoryId = updatedProducto.CategoryId;
        producto.SupplierId = updatedProducto.SupplierId;
        producto.UnidadMedida = updatedProducto.UnidadMedida;
        producto.Peso = updatedProducto.Peso;
        producto.Volumen = updatedProducto.Volumen;
        producto.Dimensiones = updatedProducto.Dimensiones;
        producto.ImagenUrl = updatedProducto.ImagenUrl;
        producto.Notas = updatedProducto.Notas;
        producto.Tags = updatedProducto.Tags;
        producto.IsActive = updatedProducto.IsActive;

        producto.UpdatedAt = DateTime.UtcNow;
        producto.UpdatedBy = "01";

        context.LineaProductos.Update(producto);
        await context.SaveChangesAsync(cancellationToken);

        return producto;
    }
}
