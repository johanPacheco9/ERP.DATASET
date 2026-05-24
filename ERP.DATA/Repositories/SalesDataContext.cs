using ERP.TRAN.CrossLayers.Core.Agreggates.Payments;
using Microsoft.EntityFrameworkCore;
namespace ERP.DATA.Repositories;

public partial class MainDataContext
{
    public DbSet<SalePayment> SalePayments { get; set; }
}