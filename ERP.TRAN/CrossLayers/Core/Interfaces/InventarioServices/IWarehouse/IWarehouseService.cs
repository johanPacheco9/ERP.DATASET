using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Requests;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;

namespace ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IWarehouse;

public interface IWarehouseService
{
    Task<bool> BodegaExistsAsync(int id, CancellationToken cancellationToken);
    Task<WarehouseDetailDTO> GetBodegaByIdAsync(int id, CancellationToken cancellationToken);
    Task<int> AddBodegaAsync(CreateBodegaRequest bodegarequest, CancellationToken cancellationToken);
    Task<bool> ExisteBodegaPorCodigoAsync(string codigo, CancellationToken cancellationToken);
    Task<WarehouseDetailDTO> UpdateBodega(UpdateWarehouseRequest request, CancellationToken cancellationToken);

    Task<PagedList<WarehouseSummaryDto>> List(ListWarehousesRequest request, CancellationToken cancellation);
};