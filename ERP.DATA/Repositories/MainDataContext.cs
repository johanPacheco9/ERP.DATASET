using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Repositories;
public partial class MainDataContext(DbContextOptions<MainDataContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

