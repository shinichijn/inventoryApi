using Microsoft.EntityFrameworkCore;
using InventoryApi.Entities;

namespace InventoryApi.Repositories
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
           : base(options)
        {

        }

        public DbSet<InventoryEntity> InventoryItems { get; set; }

    }
}
