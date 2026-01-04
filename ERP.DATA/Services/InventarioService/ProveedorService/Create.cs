namespace ERP.DATA.Services.Inventario.ProveedorService;

using ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

public partial class ProveedorService
{
    public async Task<Proveedor> AddProveedorAsync(Proveedor proveedor, CancellationToken cancellationToken)
    {
        proveedor = new Proveedor
        {
            Nombre = proveedor.Nombre,
            Nit = proveedor.Nit,
            Direccion = proveedor.Direccion,
            Telefono = proveedor.Telefono,
            Activo = proveedor.Activo,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "01"
        };
        _context.Proveedores.Add(proveedor);
        await _context.SaveChangesAsync(cancellationToken);
        return proveedor;
    }
}
