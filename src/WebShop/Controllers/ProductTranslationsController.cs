using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebShop.Models;
using Microsoft.Extensions.Localization;

namespace WebShop.Controllers
{
    public class ProductTranslationsController : Controller
    {
        private readonly WebShopRepository _context;
        
        

        public ProductTranslationsController(WebShopRepository context, IStringLocalizer<ProductTranslationsController> localizerTranslation , IStringLocalizer<ProductsController> localizer )
        {
            _context = context;
           
        }

        // GET: ProductTranslations
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProductTranslations.ToListAsync());
        }

        // GET: ProductTranslations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productTranslation = await _context.ProductTranslations.SingleOrDefaultAsync(m => m.ProductId == id);
            if (productTranslation == null)
            {
                return NotFound();
            }

            return View(productTranslation);
        }

        // GET: ProductTranslations/Create
        public IActionResult Create(int? id)
        {
            ViewData["ProductId"] = id;
            return View();
        }

        // POST: ProductTranslations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Language,ProductDescription,ProductName")] ProductTranslation productTranslation, int? id)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productTranslation);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Products");
            }
            return View(productTranslation);
        }

        // GET: ProductTranslations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productTranslation = await _context.ProductTranslations.SingleOrDefaultAsync(m => m.ProductId == id);
            if (productTranslation == null)
            {
                return NotFound();
            }
            return View(productTranslation);
        }

        // POST: ProductTranslations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Language,ProductDescription,ProductName")] ProductTranslation productTranslation)
        {
            if (id != productTranslation.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productTranslation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductTranslationExists(productTranslation.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(productTranslation);
        }

        // GET: ProductTranslations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productTranslation = await _context.ProductTranslations.SingleOrDefaultAsync(m => m.ProductId == id);
            if (productTranslation == null)
            {
                return NotFound();
            }

            return View(productTranslation);
        }

        // POST: ProductTranslations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productTranslation = await _context.ProductTranslations.SingleOrDefaultAsync(m => m.ProductId == id);
            _context.ProductTranslations.Remove(productTranslation);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ProductTranslationExists(int id)
        {
            return _context.ProductTranslations.Any(e => e.ProductId == id);
        }
    }
}
