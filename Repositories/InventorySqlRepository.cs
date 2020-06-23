using InventoryApi.Entities;
using InventoryApi.Helpers;
using InventoryApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace InventoryApi.Repositories
{
    public class InventorySqlRepository : IInventoryRepository
    {
        private readonly InventoryDbContext _inventoryDbContext;

        public InventorySqlRepository(InventoryDbContext inventoryDbContext)
        {
            _inventoryDbContext = inventoryDbContext;
        }

        public InventoryEntity GetSingle(int id)
        {
            return _inventoryDbContext.InventoryItems.FirstOrDefault(x => x.Id == id);
        }

        public void Add(InventoryEntity item)
        {
            _inventoryDbContext.InventoryItems.Add(item);
        }

        public void Delete(int id)
        {
            InventoryEntity inventoryItem = GetSingle(id);
            _inventoryDbContext.InventoryItems.Remove(inventoryItem);
        }

        public InventoryEntity Update(int id, InventoryEntity item)
        {
            _inventoryDbContext.InventoryItems.Update(item);
            return item;
        }

        public IQueryable<InventoryEntity> GetAll(QueryParameters queryParameters)
        {
            IQueryable<InventoryEntity> _allItems = _inventoryDbContext.InventoryItems.OrderBy(queryParameters.OrderBy,
              queryParameters.IsDescending());

            if (queryParameters.HasQuery())
            {
                _allItems = _allItems
                    .Where(x => x.Price.ToString().Contains(queryParameters.Query.ToLowerInvariant())
                    || x.Name.ToLowerInvariant().Contains(queryParameters.Query.ToLowerInvariant()));
            }

            return _allItems
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        public int Count()
        {
            return _inventoryDbContext.InventoryItems.Count();
        }

        public bool Save()
        {
            return (_inventoryDbContext.SaveChanges() >= 0);
        }

        public ICollection<InventoryEntity> GetRandomMeal()
        {
            List<InventoryEntity> toReturn = new List<InventoryEntity>();

            toReturn.Add(GetRandomItem("Starter"));
            toReturn.Add(GetRandomItem("Main"));
            toReturn.Add(GetRandomItem("Dessert"));

            return toReturn;
        }

        private InventoryEntity GetRandomItem(string type)
        {
            return _inventoryDbContext.InventoryItems
                .Where(x => x.Type == type)
                .OrderBy(o => Guid.NewGuid())
                .FirstOrDefault();
        }
    }
}
