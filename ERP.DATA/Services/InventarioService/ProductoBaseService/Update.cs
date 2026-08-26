using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.ProductoBaseService;

public partial class ProductoBaseService
{
     public Task<ProductoBase> UpdateProducto(int id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    public async Task<ProductoBase> UpdateProducto(int id, ProductoBase updatedProductoBase, CancellationToken cancellationToken = default)
    {
        var producto = await context.ProductoBase.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (producto == null)
            return null;
        
        producto.Name = updatedProductoBase.Name;
        producto.Description = updatedProductoBase.Description;
        producto.CostoUnitario = updatedProductoBase.CostoUnitario;
        producto.PrecioVenta = updatedProductoBase.PrecioVenta;
        producto.PorcentajeIVA = updatedProductoBase.PorcentajeIVA;
        producto.PorcentajeICA = updatedProductoBase.PorcentajeICA;
        producto.ImpuestoEspecifico = updatedProductoBase.ImpuestoEspecifico;
        producto.ArancelImportacion = updatedProductoBase.ArancelImportacion;
        producto.ExentoIVA = updatedProductoBase.ExentoIVA;
        producto.GravadoICA = updatedProductoBase.GravadoICA;
        producto.CodigoTributario = updatedProductoBase.CodigoTributario;
        producto.CategoryId = updatedProductoBase.CategoryId;
        producto.SupplierId = updatedProductoBase.SupplierId;
        producto.UnidadMedida = updatedProductoBase.UnidadMedida;
        producto.Peso = updatedProductoBase.Peso;
        producto.Volumen = updatedProductoBase.Volumen;
        producto.Dimensiones = updatedProductoBase.Dimensiones;
        producto.ImagenUrl = updatedProductoBase.ImagenUrl;
        producto.Notas = updatedProductoBase.Notas;
        producto.Tags = updatedProductoBase.Tags;
        producto.IsActive = updatedProductoBase.IsActive;

        producto.UpdatedAt = DateTime.UtcNow;
        producto.UpdatedBy = "01";

        context.ProductoBase.Update(producto);
        await context.SaveChangesAsync(cancellationToken);

        return producto;
    }
}
