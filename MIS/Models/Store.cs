using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MIS.Models
{
    public class Store
    {
        [Display(Name = "Store ID")]
        public int StoreID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Store Name")]
        public string Name { get; set; }

        public ICollection<StoreInventory> StoreInventory { get; } = new List<StoreInventory>();
    }
}
