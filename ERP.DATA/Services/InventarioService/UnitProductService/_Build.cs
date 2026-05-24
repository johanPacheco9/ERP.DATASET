using ERP.DATA.Repositories;
using Microsoft.Extensions.Logging;
using MainDataContext = ERP.DATA.Repositories.MainDataContext;

namespace ERP.DATA.Services.InventarioService.UnitProductService;

public partial class UnitProductService(ILogger<UnitProductService> logger, MainDataContext context);
