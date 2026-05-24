using ERP.DATA.Repositories;
using Microsoft.Extensions.Logging;
using MainDataContext = ERP.DATA.Repositories.MainDataContext;

namespace ERP.DATA.Services.VentasService.SaleService;

public partial class SaleService(ILogger<SaleService> logger, MainDataContext context);
