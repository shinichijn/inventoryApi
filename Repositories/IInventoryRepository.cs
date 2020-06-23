using System.Collections.Generic;
using System.Linq;
using InventoryApi.Entities;
using InventoryApi.Models;

namespace InventoryApi.Repositories
{
    public interface IInventoryRepository
    {
        InventoryEntity GetSingle(int id);
        void Add(InventoryEntity item);
        void Delete(int id);
        InventoryEntity Update(int id, InventoryEntity item);
        IQueryable<InventoryEntity> GetAll(QueryParameters queryParameters);

        ICollection<InventoryEntity> GetRandomMeal();
        int Count();

        bool Save();
    }
}
