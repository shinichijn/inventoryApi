using System;

namespace InventoryApi.Entities
{
    public class InventoryEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Price { get; set; }
        public DateTime Created { get; set; }
    }
}
