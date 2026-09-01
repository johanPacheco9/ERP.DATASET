using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Enums;

namespace ERP.DATASET.Components.Pages.Inventario.NewFolder;

public class UpdateWarehouseForm
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Location { get; set; }
    public WarehouseType Type { get; set; }
    public int StoreId { get; set; }
    public decimal? MaxCapacity { get; set; }
    public bool IsActive { get; set; }

    // ===== Factory =====
    public static UpdateWarehouseForm FromSummaryDto(WarehouseSummaryDto dto)
    {
        return new UpdateWarehouseForm
        {
            Id = dto.Id,
            Code = dto.Code,
            Name = dto.Name,
            Location = dto.Location,
            Type = dto.Type,
            StoreId = dto.StoreId,
            MaxCapacity = dto.MaxCapacity,
            IsActive = dto.IsActive
        };
    }

    // ===== To Request =====
    public UpdateWarehouseRequest ToRequest()
    {
        return new UpdateWarehouseRequest
        {
            Id = Id,
            Code = Code,
            Max_Capacity = MaxCapacity
        };
    }
}
