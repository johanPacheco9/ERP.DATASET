using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.Core.Interfaces.ReportesServices;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.ReportesService;

public partial class ReportesService(
    MainDataContext context,
    ILogger<ReportesService> logger) : IReportesService
{
    private readonly MainDataContext _context = context;
    private readonly ILogger<ReportesService> _logger = logger;
}
