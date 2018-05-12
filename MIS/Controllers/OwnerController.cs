using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIS.Data;
using MIS.Features;
using MIS.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MIS.Models.ViewModels;

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
                    ownerInventory = ownerInventory.OrderByDescending(o => o.Product.Name);
                    break;
                case "Price":
                    ownerInventory = ownerInventory.OrderBy(o => o.Product.Value);
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
                    ownerInventory = ownerInventory.OrderBy(o => o.ProductID);
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
            StoreInventoryViewModel svm = new StoreInventoryViewModel
            {
                OwnerInventories = new List<OwnerInventory>(ownerinventory),
                StockRequests = new List<StockRequest>(query)
            };
            return View(svm);
        }

        //update stock level code
        public async Task<ActionResult> UpdateStockLevel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ownerInventory = await _context.OwnerInventory
                .Include(x => x.Product)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ProductID == id);

            if (ownerInventory == null)
            {
                return NotFound();
            }

            return View(ownerInventory);
        }

        //POST:Owner/UpdateStockLevl/5
        [HttpPost, ActionName("UpdateStockLevel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStockLevelPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var productToUpdate = await _context.OwnerInventory.SingleOrDefaultAsync(o => o.ProductID == id);
            if (await TryUpdateModelAsync<OwnerInventory>(
                productToUpdate,
                "",
                o => o.StockLevel))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(productToUpdate);
        }

        //approve store request
        public async Task<ActionResult> ApproveRequest(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query = await _context.StockRequest
                .Include(x => x.Product)
                .Include(x => x.Store)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.StockRequestID == id);

            var ownerinventory = _context.OwnerInventory.Select(x => x);

            StoreInventoryViewModel svm = new StoreInventoryViewModel
            {
                StockRequests = new List<StockRequest> { query },
                OwnerInventories = new List<OwnerInventory>(ownerinventory)
            };

            return View(svm);
        }

        //POST:Owner/ApproveRequest/5
        [HttpPost, ActionName("ApproveRequest")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveRequestPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockrequest = await _context.StockRequest.AsNoTracking().FirstOrDefaultAsync(s => s.StockRequestID == id);
            var pid = stockrequest.ProductID;
            var sid = stockrequest.StoreID;
            var quantity = stockrequest.Quantity;

            await UpdateOwnerInventory(pid, quantity);
            await UpdateStoreInventory(sid, pid, quantity);
            await DeleteStockRequest(id);

            return RedirectToAction(nameof(List_Stock_Request));
        }

        public async Task UpdateStoreInventory(int sid, int pid, int quantity)
        {
            var storeInventoryToUpdate = await _context.StoreInventory.Where(s => s.ProductID == pid)
                .Where(s => s.StoreID == sid).SingleOrDefaultAsync();

            try
            {
                if (storeInventoryToUpdate == null)
                {
                    RedirectToAction(nameof(List_Stock_Request));
                }
                else
                {
                    storeInventoryToUpdate.StockLevel += quantity;
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                ModelState.AddModelError("", "Unable to save Store Inventory changes.");
            }
        }

        public async Task UpdateOwnerInventory(int pid, int quantity)
        {
            var ownerInventoryToUpdate = await _context.OwnerInventory.SingleOrDefaultAsync(o => o.ProductID == pid);

            try
            {
                if (ownerInventoryToUpdate == null)
                {
                    RedirectToAction(nameof(List_Stock_Request));
                }
                else
                {
                    ownerInventoryToUpdate.StockLevel -= quantity;
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException /* ex */)
            {
                ModelState.AddModelError("", "Unable to save Owner Inventory changes.");
            }
        }

        public async Task DeleteStockRequest(int? stockid)
        {

            // delete the current store request
            var stockRequestToDelete = await _context.StockRequest.SingleOrDefaultAsync(o => o.StockRequestID == stockid);

            try
            {
                if (stockRequestToDelete == null)
                {
                    RedirectToAction(nameof(List_Stock_Request));
                }
                else
                {
                    _context.StockRequest.Remove(stockRequestToDelete);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException /* ex */)
            {
                ModelState.AddModelError("", "Unable to save Owner Inventory changes.");
            }
        }

    }
}