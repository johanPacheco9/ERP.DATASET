using System.ComponentModel.DataAnnotations;


namespace ERP.TRAN.CrossLayers.API.Inventario.Bodega.Enums;
public enum WarehouseType
{
    [Display(Name = "Bodega de pérdidas")]
    LossWarehouse = 10,

    [Display(Name ="Bodega de bajas")]
    WriteOffWarehouse = 20,

    [Display(Name = "Bodega de recuperaciones")]
    RecoveryWarehouse = 30
}
