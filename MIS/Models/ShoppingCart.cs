using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MIS.Data;

namespace MIS.Models
{
    public class ShoppingCart
    {
        private readonly ApplicationDbContext _appDbContext;

        private ShoppingCart(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public string ShoppingCartId { get; set; }
        public List<ShoppingCartItem> ShoppingCartItems { get; set; }

        //Get Cart of current session
        public static ShoppingCart GetCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?
                .HttpContext.Session;
            var context = services.GetService<ApplicationDbContext>();
            string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();

            session.SetString("CartId",cartId);

            return new ShoppingCart(context)
            {
                ShoppingCartId = cartId
            };
        }

        //Add something to cart
        public void AddToCart(StoreInventory storeInventory, int amount)
        {
            var shoppingCartItem = _appDbContext.ShoppingCartItems
                .Where(s => s.StoreInventory.ProductID == storeInventory.ProductID)
                .Where(s => s.StoreInventory.StoreID == storeInventory.StoreID).SingleOrDefault(s => s.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem == null)
            {
                var newShoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = ShoppingCartId,
                    StoreInventory = storeInventory,
                    Amount = amount
                };

                _appDbContext.ShoppingCartItems.Add(newShoppingCartItem);
            }
            else
            {
                shoppingCartItem.Amount++;
            }

            _appDbContext.SaveChanges();
        }

        //Minus item from cart
        public int MinusFromCart(StoreInventory storeInventory)
        {
            var shoppingCartItem = _appDbContext.ShoppingCartItems
                .Where(s => s.StoreInventory.ProductID == storeInventory.ProductID)
                .Where(s => s.StoreInventory.StoreID == storeInventory.StoreID)
                .Where(s => s.ShoppingCartId == ShoppingCartId).SingleOrDefaultAsync();

            var localAmount = 0;

            if (shoppingCartItem.Result.Amount>1)
            {
                shoppingCartItem.Result.Amount--;
                localAmount = shoppingCartItem.Result.Amount;
            }
            else
            {
                _appDbContext.ShoppingCartItems.Remove(shoppingCartItem.Result);
            }

            _appDbContext.SaveChanges();
            return localAmount;
        }

        //Remove item from cart
        public void RemoveFromCart(StoreInventory storeInventory)
        {
            var shoppingCartItem = _appDbContext.ShoppingCartItems
                .Where(s => s.StoreInventory.ProductID == storeInventory.ProductID)
                .Where(s => s.StoreInventory.StoreID == storeInventory.StoreID)
                .Where(s => s.ShoppingCartId == ShoppingCartId).SingleOrDefaultAsync();

            if (shoppingCartItem != null)
            {
                _appDbContext.ShoppingCartItems.Remove(shoppingCartItem.Result);
            }

            _appDbContext.SaveChanges();
        }

        //Retrun all items in shopping cart
        public List<ShoppingCartItem> GetShoppingCartItems()
        {
            return ShoppingCartItems ??
                   (ShoppingCartItems = _appDbContext.ShoppingCartItems.Where(c => c.ShoppingCartId == ShoppingCartId)
                       .Include(s => s.StoreInventory)
                       .Include(s=>s.StoreInventory.Store)
                       .Include(s => s.StoreInventory.Product)
                       .ToList());
        }

        //clear the cart
        public void ClearCart()
        {
            var cartItems = _appDbContext.ShoppingCartItems.Where(cart => cart.ShoppingCartId == ShoppingCartId);
            _appDbContext.ShoppingCartItems.RemoveRange(cartItems);
            _appDbContext.SaveChangesAsync();
        }

        //return the total amout of current cart
        public double GetShoppingCartTotal()
        {
            var total = _appDbContext.ShoppingCartItems
                .Where(c => c.ShoppingCartId == ShoppingCartId)
                .Select(c => c.Amount * c.StoreInventory.Product.Value).Sum();

            return total;
        }
    }
}
