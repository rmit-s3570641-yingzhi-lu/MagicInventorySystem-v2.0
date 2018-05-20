using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIS.Data;
using MIS.Models;
using MIS.Utilities;

namespace MIS.Controllers
{
    public class PaymentController : Controller
    {

        private readonly ShoppingCart _shoppingCart;
        private readonly ApplicationDbContext _context;

        public PaymentController(ShoppingCart shoppingCart, ApplicationDbContext context)
        {
            _shoppingCart = shoppingCart;
            _context = context;
        }

        public ActionResult Index()
        {
            // The default card type selected is master card.
            return View(new CreditCardForm { CreditCardType = CardType.MasterCard });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(CreditCardForm creditCardForm)
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

            //update the store inventory
            var shoppingCartItem = _shoppingCart.GetShoppingCartItems();
            foreach (var item in shoppingCartItem)
            {
                UpdateStoreInventory(item);
            }

            //clear shopping cart after pay
            await _shoppingCart.ClearCartAsync();

            
            return View("PaymentReceived");
        }

        //Update store inventory
        public void UpdateStoreInventory(ShoppingCartItem shoppingCartItem)
        {
            var currSid = shoppingCartItem.StoreInventory.StoreID;
            var currPid = shoppingCartItem.StoreInventory.ProductID;
            var amount = shoppingCartItem.Amount;

            var itemToUpdate = _context.StoreInventory.Where(s => s.StoreID == currSid)
                .Where(s => s.ProductID == currPid).SingleOrDefaultAsync();

            itemToUpdate.Result.StockLevel -= amount;

            _context.SaveChanges();
        }
    }
}