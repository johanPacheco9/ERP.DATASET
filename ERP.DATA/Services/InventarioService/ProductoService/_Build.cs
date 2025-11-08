using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.Inventario.ProductoService;

/// <summary>
/// Servicio de gestión de productos
/// </summary>
public partial class ProductoService : IProductoService
{
    private readonly ILogger<ProductoService> _logger;
    private readonly MainDataContext _context;

    public ProductoService(ILogger<ProductoService> logger, MainDataContext context)
    {
        _logger = logger;
        _context = context;
    }
}
