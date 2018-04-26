using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MIS.Data;
using Microsoft.AspNetCore.Authorization;

namespace MIS.Controllers
{
    [Authorize(Roles = Constants.FranchiseHolderRole)]
    public class FranchiseController : Controller
    {
        // GET: Franchise
        public ActionResult Index()
        {
            return View();
        }

        // GET: Franchise/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Franchise/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Franchise/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Franchise/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Franchise/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Franchise/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Franchise/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}