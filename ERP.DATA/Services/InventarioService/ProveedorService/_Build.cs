using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IProveedores;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.Inventario.ProveedorService;

/// <summary>
/// Servicio de gestión de productos
/// </summary>
public partial class ProveedorService : IProveedorService
{
    private readonly ILogger<ProveedorService> _logger;
    private readonly MainDataContext _context;

    public ProveedorService(ILogger<ProveedorService> logger, MainDataContext context)
    {
        _logger = logger;
        _context = context;
    }
}
