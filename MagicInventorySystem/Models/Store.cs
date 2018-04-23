﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MagicInventorySystem.Models
{
    public class Store
    {
        [Display(Name = "Store ID")]
        public int StoreID { get; set; }
        public string Name { get; set; }

        public ICollection<StoreInventory> StoreInventory { get; } = new List<StoreInventory>();
    }
}
