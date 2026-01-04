
using ERP.DATA.Repositories;
using ERP.DATA.Services.Inventario.ProductoService;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Request;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IProductosVariantes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.ProductoVarianteService;

public partial class ProductoVarianteService : IProductoVarianteService
{

    private readonly ILogger<ProductoVarianteService> _logger;
    private readonly MainDataContext _context;

    public ProductoVarianteService(ILogger<ProductoVarianteService> logger, MainDataContext context)
    {
        _logger = logger;
        _context = context;
    }
}
