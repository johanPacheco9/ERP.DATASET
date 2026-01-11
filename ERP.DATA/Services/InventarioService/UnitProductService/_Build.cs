using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IUnitProduct;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.UnitProductService;

public partial class UnitProductService : IUnitProductService
{
    private readonly ILogger<UnitProductService> _logger;
    private readonly MainDataContext _context;

    public UnitProductService(ILogger<UnitProductService> logger, MainDataContext context)
    {
        _logger = logger;
        _context = context;
    }
}
