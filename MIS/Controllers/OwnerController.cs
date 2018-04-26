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

        //public const string SessionKeyStoreID = "_StoreID";
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

            query = query.OrderBy(x => x.Product.Name);

            return View(await query.ToListAsync());
        }
    }
}

// Set id into session.
//HttpContext.Session.SetInt32(SessionKeyStoreID, id.Value);
// Get id from session.
//var id = HttpContext.Session.GetInt32(SessionKeyStoreID);
//            if(id == null)
//            {
//                return BadRequest();
//            }