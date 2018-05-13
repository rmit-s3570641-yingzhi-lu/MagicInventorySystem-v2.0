using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MIS.Models
{
    public class ShoppingCartItem
    {
        public int ShoppingCartItemID { get; set; }
        public StoreInventory StoreInventory { get; set; }
        public int Amount { get; set; }
        public string ShoppingCartId { get; set; }
    }
}
