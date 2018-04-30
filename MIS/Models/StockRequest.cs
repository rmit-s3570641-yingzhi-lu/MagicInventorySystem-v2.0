using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIS.Models
{
    public class StockRequest
    {
        [Display(Name = "Stock Request ID")]
        public int StockRequestID { get; set; }

        public int StoreID { get; set; }
        public Store Store { get; set; }

        public int ProductID { get; set; }
        public Product Product { get; set; }

        [Range(1,100,ErrorMessage ="Mush Below 100")]
        public int Quantity { get; set; }
    }
}
