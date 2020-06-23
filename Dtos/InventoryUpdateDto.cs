using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryApi.Dtos
{
    public class InventoryUpdateDto
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public string Type { get; set; }
        public DateTime Created { get; set; }
    }
}
