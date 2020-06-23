using System;

namespace InventoryApi.Dtos
{
    public class InventoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Price { get; set; }
        public DateTime Created { get; set; }
    }
}
