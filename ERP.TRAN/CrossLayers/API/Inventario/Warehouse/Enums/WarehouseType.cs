using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Enums;
public enum WarehouseType
{
    [Display(Name = "Bodega principal")]
    Principal = 10,

    [Display(Name = "Warehouse de pérdidas")]
    LossWarehouse = 15,

    [Display(Name ="Warehouse de bajas")]
    WriteOffWarehouse = 20,

    [Display(Name = "Warehouse de recuperaciones")]
    RecoveryWarehouse = 30
}
