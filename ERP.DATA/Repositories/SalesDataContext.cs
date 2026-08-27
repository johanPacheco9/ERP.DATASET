using ERP.TRAN.CrossLayers.Core.Agreggates.Payments;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;
using Microsoft.EntityFrameworkCore;
namespace ERP.DATA.Repositories;

public partial class MainDataContext
{
    public DbSet<SalePayment> SalePayments { get; set; }
    public DbSet<PosShift>  PosShifts { get; set; }
    public DbSet<PosShiftMovement> PosShiftMovements { get; set; }
    public DbSet<PosTerminal>  PosTerminals { get; set; }
}