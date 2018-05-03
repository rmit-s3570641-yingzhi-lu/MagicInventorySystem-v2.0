using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MIS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MIS.Models;

namespace MIS.Controllers
{
    [Authorize(Roles = Constants.FranchiseHolderRole)]
    public class FranchiseController : Controller
    {
        private readonly UserManager<ApplicationUser>  _userManager;
        private readonly ApplicationDbContext _context;

        public FranchiseController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Franchise
        public async Task<ActionResult> Index(string productName)
        {
            var user = await _userManager.GetUserAsync(User);

            //user.Store.StoreInventory

            var query = _context.StoreInventory.Include(x => x.Product)
                                    .Include(s => s.Store).Where(x => x.StoreID == user.StoreID).Select(x => x);

            if (!string.IsNullOrWhiteSpace(productName))
            {
                query = query.Where(x => x.Product.Name.Contains(productName));
                ViewBag.ProductName = productName;
            }

            query = query.OrderBy(x => x.StockLevel);

            return View(await query.ToListAsync());
        }

        public async Task<ActionResult> Create_Existing(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);

            var storeInventory = await _context.StoreInventory
                .Include(x => x.Product)
                .Include(s => s.Store)
                .Where(x => x.StoreID == user.StoreID)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ProductID == id);

            if (storeInventory == null)
            {
                return NotFound();
            }

            return View(storeInventory);
        }

        //POST:Owner/UpdateStockLevl/5
        [HttpPost, ActionName("Create_Existing")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_ExistingPost(int quantity,int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var productToUpdate = await _context.StoreInventory.SingleOrDefaultAsync(s=>s.ProductID == id);

            try
            {
                if (ModelState.IsValid)
                {
                    var stockRequest = new StockRequest()
                    {
                        ProductID = productToUpdate.ProductID,
                        Quantity = quantity,
                        StoreID = (int)user.StoreID
                    };

                    _context.Add(stockRequest);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            return View(productToUpdate);
        }

    }
}