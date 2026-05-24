
using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IProductVariant;
using Microsoft.Extensions.Logging;
using MainDataContext = ERP.DATA.Repositories.MainDataContext;

namespace ERP.DATA.Services.InventarioService.ProductoVarianteService;

public partial class ProductVariantService
{

    private readonly ILogger<ProductVariantService> _logger;
    private readonly MainDataContext _context;

    public ProductVariantService(ILogger<ProductVariantService> logger, MainDataContext context)
    {
        _logger = logger;
        _context = context;
    }
}
