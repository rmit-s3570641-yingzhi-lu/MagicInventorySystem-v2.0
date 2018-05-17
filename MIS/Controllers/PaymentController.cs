using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MIS.Models;
using MIS.Utilities;

namespace MIS.Controllers
{
    public class PaymentController : Controller
    {
        public ActionResult Index()
        {
            // The default card type selected is master card.
            return View(new CreditCardForm { CreditCardType = CardType.MasterCard });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(CreditCardForm creditCardForm)
        {
            // Validate card type.
            CardType expectedCardType = CardTypeInfo.GetCardType(creditCardForm.CreditCardNumber);
            if (expectedCardType == CardType.Unknown || expectedCardType != creditCardForm.CreditCardType)
            {
                ModelState.AddModelError("CreditCardType", "The Credit Card Type field does not match against the credit card number.");
            }

            if (!ModelState.IsValid)
            {
                return View(creditCardForm);
            }

            return View("PaymentReceived");
        }
    }
}