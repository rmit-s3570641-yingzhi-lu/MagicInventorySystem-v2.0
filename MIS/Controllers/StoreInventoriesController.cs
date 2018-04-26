using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MIS.Data;
using MIS.Models;

namespace MIS.Controllers
{
    public class StoreInventoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StoreInventoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StoreInventories
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StoreInventory.Include(s => s.Product).Include(s => s.Store);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: StoreInventories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeInventory = await _context.StoreInventory
                .Include(s => s.Product)
                .Include(s => s.Store)
                .SingleOrDefaultAsync(m => m.StoreID == id);
            if (storeInventory == null)
            {
                return NotFound();
            }

            return View(storeInventory);
        }

        // GET: StoreInventories/Create
        public IActionResult Create()
        {
            ViewData["ProductID"] = new SelectList(_context.Product, "ProductID", "ProductID");
            ViewData["StoreID"] = new SelectList(_context.Store, "StoreID", "StoreID");
            return View();
        }

        // POST: StoreInventories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StoreID,ProductID,StockLevel")] StoreInventory storeInventory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(storeInventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductID"] = new SelectList(_context.Product, "ProductID", "ProductID", storeInventory.ProductID);
            ViewData["StoreID"] = new SelectList(_context.Store, "StoreID", "StoreID", storeInventory.StoreID);
            return View(storeInventory);
        }

        // GET: StoreInventories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeInventory = await _context.StoreInventory.SingleOrDefaultAsync(m => m.StoreID == id);
            if (storeInventory == null)
            {
                return NotFound();
            }
            ViewData["ProductID"] = new SelectList(_context.Product, "ProductID", "ProductID", storeInventory.ProductID);
            ViewData["StoreID"] = new SelectList(_context.Store, "StoreID", "StoreID", storeInventory.StoreID);
            return View(storeInventory);
        }

        // POST: StoreInventories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StoreID,ProductID,StockLevel")] StoreInventory storeInventory)
        {
            if (id != storeInventory.StoreID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(storeInventory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreInventoryExists(storeInventory.StoreID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductID"] = new SelectList(_context.Product, "ProductID", "ProductID", storeInventory.ProductID);
            ViewData["StoreID"] = new SelectList(_context.Store, "StoreID", "StoreID", storeInventory.StoreID);
            return View(storeInventory);
        }

        // GET: StoreInventories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeInventory = await _context.StoreInventory
                .Include(s => s.Product)
                .Include(s => s.Store)
                .SingleOrDefaultAsync(m => m.StoreID == id);
            if (storeInventory == null)
            {
                return NotFound();
            }

            return View(storeInventory);
        }

        // POST: StoreInventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var storeInventory = await _context.StoreInventory.SingleOrDefaultAsync(m => m.StoreID == id);
            _context.StoreInventory.Remove(storeInventory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StoreInventoryExists(int id)
        {
            return _context.StoreInventory.Any(e => e.StoreID == id);
        }
    }
}
