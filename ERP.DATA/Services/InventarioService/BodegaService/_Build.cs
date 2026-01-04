using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IBodegas;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.BodeegaService;

public partial class BodegaService : IBodegaService
{
    private readonly ILogger<BodegaService> _logger;
    private readonly MainDataContext _context;

    public BodegaService(ILogger<BodegaService> logger, MainDataContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<bool> BodegaExistsAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Bodegas
            .AnyAsync(b => b.Id == id, cancellationToken);
    }

    public Task<bool> ExisteBodegaPorCodigoAsync(string codigo, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}


