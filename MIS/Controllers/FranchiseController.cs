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
    [Authorize(Roles = Constants.FranchiseHolderRole)]
    public class FranchiseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FranchiseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Franchise
        public async Task<ActionResult> Index(string productName)
        {
            var query = _context.StoreInventory.Include(x => x.Product)
                                    .Include(s => s.Store).Where(x => x.StoreID == 1).Select(x => x);

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

            var storeInventory = await _context.StoreInventory
                .Include(x => x.Product)
                .Include(s => s.Store)
                .Where(x => x.StoreID == 1)
                .SingleOrDefaultAsync(m => m.ProductID == id);

            if (storeInventory == null)
            {
                return NotFound();
            }

            return View(storeInventory);
        }
    }
}