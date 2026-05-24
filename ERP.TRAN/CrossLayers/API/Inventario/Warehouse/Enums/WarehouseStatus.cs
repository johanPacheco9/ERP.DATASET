using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Enums;

public enum WarehouseStatus
{

    [Display(Name = "En inventario")]
    OnInventory = 10,

    [Display(Name = "Cerrada")]
    Closed = 10,

    [Display(Name = "Capacidad máxima")]
    Fully = 20,

    [Display(Name = "En auditoría")]
    Onaudit = 30
}

