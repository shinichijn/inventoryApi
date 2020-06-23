using InventoryApi.Entities;
using InventoryApi.Repositories;
using System;
using System.Threading.Tasks;

namespace InventoryApi.Services
{
    public class SeedDataService : ISeedDataService
    {
        public async Task Initialize(InventoryDbContext context)
        {
            context.InventoryItems.Add(new InventoryEntity() { Price = 1000, Type = "Starter", Name = "Lasagne", Created = DateTime.Now });
            context.InventoryItems.Add(new InventoryEntity() { Price = 1100, Type = "Main", Name = "Hamburger", Created = DateTime.Now });
            context.InventoryItems.Add(new InventoryEntity() { Price = 1200, Type = "Dessert", Name = "Spaghetti", Created = DateTime.Now });
            context.InventoryItems.Add(new InventoryEntity() { Price = 1300, Type = "Starter", Name = "Pizza", Created = DateTime.Now });

            await context.SaveChangesAsync();
        }
    }
}
