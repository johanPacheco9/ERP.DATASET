using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.ICategorias;
using Microsoft.Extensions.Logging;
using MainDataContext = ERP.DATA.Repositories.MainDataContext;

namespace ERP.DATA.Services.InventarioService.CategoriaService;

public partial class CategoriaService(ILogger<CategoriaService> logger, MainDataContext context)
{
    
}
