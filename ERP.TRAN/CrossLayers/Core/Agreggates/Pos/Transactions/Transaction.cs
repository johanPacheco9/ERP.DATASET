
using ERP.TRAN.CrossLayers.API.Pos.Transactions.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.Stores;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Transactions;

public class Transaction : EntityWithtraceability
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public List<Producto> productos { get; set; } 

    public Store Store { get; set; } = null!;
    public TransactionStatus TransactionStatus { get; set; }
}
