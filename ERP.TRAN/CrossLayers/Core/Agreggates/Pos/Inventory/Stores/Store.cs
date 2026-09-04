using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.Stores;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.Stores;

public class Store : EntityWithtraceability
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsMainStore{ get; set; }
    // public StoreType Type { get; set; } 
     // clasifica la tienda (Principal, Sucursal, Bodega, Online, etc.)
    public StoreType Type { get; set; } = StoreType.Sucursal;

    public ICollection<Warehouse> Bodegas { get; set; } = new List<Warehouse>();
    
    /// <summary>
    /// Cajas registradas en la tienda.
    /// </summary>
    public ICollection<PosTerminal> Cajas { get; set; } = new List<PosTerminal>();
    
    //// <summary>
    /// Colección de asociaciones con tiendas/sucursales (Relación Muchos a Muchos).
    /// </summary>
    /// <remarks>
    /// Permite que un empleado, cajero o administrador tenga permisos para operar o 
    /// supervisar múltiples sucursales bajo circunstancias de rotación de personal, 
    /// apoyo temporal o gestión multi-tienda.
    /// </remarks>
    public ICollection<UsuarioStore> UserStores { get; set; } = new List<UsuarioStore>();
}
