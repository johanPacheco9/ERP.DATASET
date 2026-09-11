using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Requests;

namespace ERP.DATASET.Components.Pages.Inventario.Warehouse;

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
    public string? Description { get; set; }
    public string? Ubication  { get; set; }
    
}
