using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MIS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MIS.Features;
using MIS.Models;

namespace MIS.Controllers
{
    //[Authorize(Roles = Constants.CustomerRole)]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

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

        public async Task<IActionResult> List(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? page, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewData["CurrentSort"] = sortOrder;
            ViewData["IDSortParm"] = String.IsNullOrEmpty(sortOrder) ? "ID_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
            ViewData["StockSortParm"] = sortOrder == "Stock" ? "stock_desc" : "Stock";

            //pagination and search
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var storeinventory = _context.StoreInventory.Include(x => x.Product).Where(x => x.StoreID == id).Distinct();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                storeinventory = storeinventory.Where(x => x.Product.Name.Contains(searchString));
            }

            //switch for ordering
            switch (sortOrder)
            {
                case "ID_desc":
                    storeinventory = storeinventory.OrderByDescending(o => o.ProductID);
                    break;
                case "Name":
                    storeinventory = storeinventory.OrderBy(o => o.Product.Name);
                    break;
                case "name_desc":
                    storeinventory = storeinventory.OrderByDescending(o => o.Product.Name);
                    break;
                case "Stock":
                    storeinventory = storeinventory.OrderBy(o => o.StockLevel);
                    break;
                case "stock_desc":
                    storeinventory = storeinventory.OrderByDescending(o => o.StockLevel);
                    break;
                default:
                    storeinventory = storeinventory.OrderBy(o => o.ProductID);
                    break;
            }

            //pagination code
            int pageSize = 5;
            return View(await PaginatedList<StoreInventory>
                .CreateAsync(storeinventory.AsNoTracking(), page ?? 1, pageSize));
        }
    }
}