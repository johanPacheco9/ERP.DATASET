
using ERP.TRAN.CrossLayers.API.Pos.Transactions.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.Stores;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Transactions;

public class Transaction : EntityWithtraceability
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public List<ProductoBase> productos { get; set; } 

    public Store Store { get; set; } = null!;
    public TransactionStatus TransactionStatus { get; set; }
}
