using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIS.Data;
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
        public async Task<IActionResult> Index(string productName)
        {

            var query = _context.OwnerInventory.Include(x => x.Product).Select(x => x);

            if (!string.IsNullOrWhiteSpace(productName))
            {
                query = query.Where(x => x.Product.Name.Contains(productName));
                ViewBag.ProductName = productName;
            }

            query = query.OrderBy(x => x.StockLevel);

            return View(await query.ToListAsync());
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