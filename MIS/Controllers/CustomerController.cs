using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MIS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace MIS.Controllers
{
    [Authorize(Roles = Constants.CustomerRole)]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        public const string SessionKeyStoreID = "_StoreID";

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customer
        public async Task<ActionResult> Index()
        {
            var stores = _context.Store.Select(x => x).OrderBy(x => x.StoreID);

            return View(await stores.ToListAsync());
        }

        public IActionResult List(int? id, string productName)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeinventory = _context.StoreInventory.Include(x => x.Product).Where(x => x.StoreID == id).Distinct();

            if (!string.IsNullOrWhiteSpace(productName))
            {
                storeinventory = storeinventory.Where(x => x.Product.Name.Contains(productName));
                ViewBag.ProductName = productName;
            }

            storeinventory = storeinventory.OrderBy(x => x.ProductID);

            if (storeinventory == null)
            {
                return NotFound();
            }

            return View(storeinventory);

        }
    }
}