using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Repositories;
public partial class MainDataContext : DbContext
{
    public MainDataContext(DbContextOptions<MainDataContext> options) : base(options)
    {
    }
}

