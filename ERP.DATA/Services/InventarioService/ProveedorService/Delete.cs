namespace ERP.DATA.Services.Inventario.ProveedorService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class ProveedorService
{
    public async Task<bool> DeleteProveedorById(int id, CancellationToken cancellationToken = default)
    {
        var proveedor = await _context.Proveedores.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (proveedor == null)
            return false;
        _context.Proveedores.Remove(proveedor);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
