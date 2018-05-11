using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MIS.Models.ViewModels
{
    public class StoreInventoryViewModel
    {
        public List<StockRequest> StockRequests { get; set; }
        public List<OwnerInventory> OwnerInventories { get; set; }
    }
}
