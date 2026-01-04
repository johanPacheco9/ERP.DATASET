using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.ICategorias;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.CategoriaService;

public partial class CategoriaService : ICategoriaService
{
    private readonly ILogger<CategoriaService> _logger;
    private readonly MainDataContext _context;

    public CategoriaService(ILogger<CategoriaService> logger, MainDataContext context)
    {
        _logger = logger;
        _context = context;
    }
    public Task<bool> DeleteCategoriaAsync(int id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    public Task<Categoria> UpdateCategoriaAsync(Categoria categoria, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
