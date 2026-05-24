using ERP.DATA.Repositories;
using Microsoft.Extensions.Logging;
using MainDataContext = ERP.DATA.Repositories.MainDataContext;

namespace ERP.DATA.Services.VentasService.ClientService;

public partial class ClientService(ILogger<ClientService> logger, MainDataContext context);
