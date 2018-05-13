using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIS.Data;
using MIS.Models;
using MIS.Models.ViewModels;

namespace MIS.Controllers
{
    [Authorize(Roles = Constants.CustomerRole)]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ShoppingCart _shoppingCart;

        public ShoppingCartController(ApplicationDbContext context, ShoppingCart shoppingCart)
        {
            _context = context;
            _shoppingCart = shoppingCart;
        }

        public ViewResult Index()
        {
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            var sCvm = new ShoppingCartViewModel
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal()
            };

            return View(sCvm);
        }

        public RedirectToActionResult AddToShoppingCart(int storeId, int productId)
        {
            var selectedProduct = _context.StoreInventory
                .Where(s => s.ProductID == productId).FirstOrDefault(s => s.StoreID == storeId);

            if (selectedProduct != null)
            {
                _shoppingCart.AddToCart(selectedProduct,1);
            }

            return RedirectToAction("Index");
        }

        public RedirectToActionResult RemoveFromShoppingCart(int storeId, int productId)
        {
            var selectedProduct = _context.StoreInventory
                .Where(s => s.ProductID == productId).FirstOrDefault(s => s.StoreID == storeId);

            if (selectedProduct != null)
            {
                _shoppingCart.RemoveFromCart(selectedProduct);
            }

            return RedirectToAction("Index");
        }

    }
}