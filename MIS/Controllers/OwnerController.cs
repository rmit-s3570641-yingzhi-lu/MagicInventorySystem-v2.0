using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIS.Data;
using MIS.Features;
using MIS.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MIS.Controllers
{
    [Authorize(Roles = Constants.OwnerRole)]
    public class OwnerController : Controller
    {

        private readonly ApplicationDbContext _context;

        public OwnerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Owner
        // GET: OwnerInventories
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["IDSortParm"] = String.IsNullOrEmpty(sortOrder) ? "ID_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
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

            var ownerInventory = _context.OwnerInventory.Include(x => x.Product).Select(x => x);                                          

            if (!String.IsNullOrEmpty(searchString))
            {
                ownerInventory = ownerInventory.Where(x => x.Product.Name.Contains(searchString));
            }

            //switch for ordering
            switch (sortOrder)
            {
                case "ID_desc":
                    ownerInventory = ownerInventory.OrderByDescending(o => o.ProductID);
                    break;
                case "Name":
                    ownerInventory = ownerInventory.OrderBy(o => o.Product.Name);
                    break;
                case "name_desc":
                    ownerInventory = ownerInventory.OrderByDescending(o=>o.Product.Name);
                    break;
                case "Price":
                    ownerInventory = ownerInventory.OrderBy(o=>o.Product.Value);
                    break;
                case "price_desc":
                    ownerInventory = ownerInventory.OrderByDescending(o => o.Product.Value);
                    break;
                case "Stock":
                    ownerInventory = ownerInventory.OrderBy(o => o.StockLevel);
                    break;
                case "stock_desc":
                    ownerInventory = ownerInventory.OrderByDescending(o => o.StockLevel);
                    break;
                default:
                    ownerInventory = ownerInventory.OrderBy(o=>o.ProductID);
                    break;
            }

            //pagination code
            int pageSize = 5;
            return View(await PaginatedList<OwnerInventory>
                .CreateAsync(ownerInventory.AsNoTracking(), page ?? 1, pageSize));
        }

        public async Task<IActionResult> List_Stock_Request(string productName)
        {
            var query = _context.StockRequest.Include(x => x.Product).Include(x => x.Store).Select(x => x);

            if (!string.IsNullOrWhiteSpace(productName))
            {
                query = query.Where(x => x.Product.Name.Contains(productName));
                ViewBag.ProductName = productName;
            }

            var ownerinventory = _context.OwnerInventory.Select(x => x);
            return View(await query.ToListAsync());
        }
    }
}