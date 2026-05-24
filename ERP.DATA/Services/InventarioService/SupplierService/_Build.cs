using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.ISupplier;
using Microsoft.Extensions.Logging;
using MainDataContext = ERP.DATA.Repositories.MainDataContext;

namespace ERP.DATA.Services.Inventario.ProveedorService;

/// <summary>
/// Servicio de gestión de proveedores
/// </summary>
public partial class SupplierService(ILogger<SupplierService> logger, MainDataContext context)
{
    private readonly ILogger<SupplierService> _logger = logger;
}
