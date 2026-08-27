using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Repositories;
public partial class MainDataContext(DbContextOptions<MainDataContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

