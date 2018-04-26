using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIS.Models
{
    public class Product
    {
        public int ProductID { get; set; }

        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [Display(Name = "Product Price")]
        public double Value { get; set; }
    }
}
