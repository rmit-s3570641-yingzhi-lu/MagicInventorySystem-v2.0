using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIS.Models
{
    public class Product
    {
        [Display(Name = "Product ID")]
        public int ProductID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [Display(Name = "Product Price")]
        [Range(1, 500), DataType(DataType.Currency)]
        public double Value { get; set; }
    }
}
