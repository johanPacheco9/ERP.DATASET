
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;

namespace ERP.DATA.Services.InventarioService.BodeegaService;

public partial class BodegaService
{
    public async Task<BodegaDetailDTO> UpdateBodega(UpdateBodegaRequest request, CancellationToken cancellationToken)
    {
        var bodega = _context.Bodegas.Where(s=>s.Id == request.Id).FirstOrDefault();
        if (bodega == null)
            throw new InvalidOperationException($"No existe bodega con ID {request.Id}");
        await _context.SaveChangesAsync();
        return new BodegaDetailDTO(bodega.Id, bodega.Nombre,bodega.Descripcion, bodega.Ubicacion,bodega.IsActive,bodega.CreatedAt,bodega.UpdatedAt);
    }
}
