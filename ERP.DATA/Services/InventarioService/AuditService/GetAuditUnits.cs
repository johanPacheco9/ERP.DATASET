using ERP.DATA.Services.InventarioService.AuditService.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Enums;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.AuditService;

public partial class AuditoriaService
{
    /// <summary>
    /// Retorna la lista de unidades asociadas a una auditoría para mostrarlas en la tabla.
    /// Conteo ciego: solo se devuelven las líneas que ya fueron tocadas por un escaneo
    /// (Found, ExcessProduct, o cualquier otro estado distinto de NotFound). Las unidades
    /// que aún no han sido escaneadas permanecen ocultas para no revelar el listado esperado.
    /// </summary>
    public async Task<List<AuditUnitDto>> GetAuditUnits(int auditId, CancellationToken cancellationToken)
    {
        return await _context.UnitProductAudits
            .Where(u => u.AuditId == auditId && u.Status != UnitProductAuditStatus.NotFound)
            .Include(u => u.ProductoVariante.UnidadesFisicas) // Ajusta si la propiedad de navegación se llama distinto en tu entidad
            .Select(u => new AuditUnitDto
            {
                Id = u.Id,
                UnidadProductoId = u.UnitProductId,
                Serial = u.Serial ?? u.ProductoVariante.CodigoBarras, // O la propiedad que tenga el código/serial
                ProductoName = u.ProductoVariante.ProductoBase.Name,
                // Ajusta al nombre real de la propiedad del producto
                ProductoVariantId = u.ProductoVarianteId,
                StatusCode = u.Status,
                StatusDisplay = u.Status.GetDisplayName(),
                UbicacionFisica = u.UbicacionFisica,
                Observaciones = u.Observaciones
            })
            .ToListAsync(cancellationToken);
    }
}