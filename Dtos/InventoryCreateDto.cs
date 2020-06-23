using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryApi.Dtos
{
    public class InventoryCreateDto
    {
        [Required]
        public string Name { get; set; }
        public string Type { get; set; }
        public int Price { get; set; }
        public DateTime Created { get; set; }
    }
}
