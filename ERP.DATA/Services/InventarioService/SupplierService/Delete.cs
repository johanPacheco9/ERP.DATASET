namespace ERP.DATA.Services.Inventario.ProveedorService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class SupplierService
{
    public async Task<bool> DeleteProveedorById(int id, CancellationToken cancellationToken = default)
    {
        var proveedor = await context.Supplier.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (proveedor == null)
            return false;
        context.Supplier.Remove(proveedor);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
