using System.ComponentModel.DataAnnotations;

namespace MagicInventorySystem.Models
{
    public class StoreInventory
    {
        public int StoreID { get; set; }
        public Store Store { get; set; }

        public int ProductID { get; set; }
        public Product Product { get; set; }

        [Display(Name = "Current Stock")]
        public int StockLevel { get; set; }
    }
}
