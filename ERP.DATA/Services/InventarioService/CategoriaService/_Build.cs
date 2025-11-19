using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
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
    public Task<bool> DeleteCategoriaAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<Categoria>> GetAllCategoriasAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Categoria> GetCategoriaByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Categoria> UpdateCategoriaAsync(Categoria categoria, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
