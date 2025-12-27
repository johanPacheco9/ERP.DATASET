using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.BodegasInventary;

namespace ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IBodegas;

public interface IBodegaService
{
    Task<bool> BodegaExistsAsync(int id, CancellationToken cancellationToken);
    Task<BodegaDetailDTO> GetBodegaByIdAsync(int id, CancellationToken cancellationToken);
    Task<int> AddBodegaAsync(CreateBodegaRequest bodegarequest, CancellationToken cancellationToken);
    Task<bool> ExisteBodegaPorCodigoAsync(string codigo, CancellationToken cancellationToken);
}