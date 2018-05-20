using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public RedirectToActionResult AddToShoppingCart(int amount, int storeId, int productId, int quantity)
        {
            var selectedProduct = _context.StoreInventory
                .Where(s => s.ProductID == productId).FirstOrDefault(s => s.StoreID == storeId);

            if (selectedProduct == null) return RedirectToAction("Index");

            var maxAmount = selectedProduct.StockLevel;
            var currentAmount = amount + quantity;
            if (maxAmount >= currentAmount)
            {
                _shoppingCart.AddToCart(selectedProduct, quantity);
            }
            else
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        public RedirectToActionResult NewAddToShoppingCart(int storeId, int productId, int quantity)
        {
            var selectedProduct = _context.StoreInventory
                .Where(s => s.ProductID == productId).FirstOrDefault(s => s.StoreID == storeId);

            if (selectedProduct != null)
            {
                _shoppingCart.AddToCart(selectedProduct, quantity);
            }

            return RedirectToAction("Index");
        }

        public RedirectToActionResult MinusFromShoppingCart(int storeId, int productId)
        {
            var selectedProduct = _context.StoreInventory
                .Where(s => s.ProductID == productId).FirstOrDefault(s => s.StoreID == storeId);

            if (selectedProduct != null)
            {
                _shoppingCart.MinusFromCart(selectedProduct);
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

        public ViewResult Orders()
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
    }
}