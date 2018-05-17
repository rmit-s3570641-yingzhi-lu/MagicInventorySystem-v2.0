using MIS.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MIS.Models
{
    public class CreditCardForm
    {
        [Display(Name = "Credit Card Type")]
        [Required]
        public CardType CreditCardType { get; set; }

        [Display(Name = "Credit Card Number")]
        [Required]
        [CreditCard]
        public string CreditCardNumber { get; set; }
    }
}
