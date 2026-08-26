using Microsoft.Extensions.Logging;
using MainDataContext = ERP.DATA.Repositories.MainDataContext;

namespace ERP.DATA.Services.InventarioService.UnidadProductoService;

public partial class UnidadProductoManager(ILogger<UnidadProductoManager> logger, MainDataContext context);
