using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DiplomaApp.Data;
using DiplomaApp.Models;
using DiplomaApp.WebScraper;
using DiplomaApp.ViewModels;
using DiplomaApp.Core;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;

namespace DiplomaApp.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index(string name, string marketplace, int page=1)
        {

            if (_context == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Category'  is null.");
            }

            var categories = _context.Category.AsEnumerable();
            if(!String.IsNullOrEmpty(name))
            {
                categories = categories.Where(c => c.Name.ToLower().Contains(name.ToLower()));
            }
            if (!String.IsNullOrEmpty(marketplace))
            {
                categories = categories.Where(c => _context.Marketplace.Find(c.MarketplaceId).Name.ToLower().Contains(marketplace.ToLower()));
            }
            List<CategoryViewModel> categoriesVM = new List<CategoryViewModel>();
            foreach(var category in categories)
            {
                var categoryMarketplace = _context.Marketplace.Find(category.MarketplaceId); 
                categoriesVM.Add(new CategoryViewModel(category, categoryMarketplace));
            }
            var viewModel = new CategoryIndex(categoriesVM.ToPagedList(page, Constants.recordsPerPage));
            return View(viewModel);
        }
        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int id, string name, int minPrice, int maxPrice, int page = 1, string order = "checkdate-desc", int similiarityRatio = Constants.defaultSimiliarityRatio)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = _context.Category.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            var offers = _context.Offer.Where(c => c.CategoryId == id).AsEnumerable();
            if (maxPrice != 0)
            {
                offers = offers.Where(c => (c.Price <= maxPrice));
            }
            if (minPrice != 0)
            {
                offers = offers.Where(c => (c.Price >= minPrice));
            }
            if (!String.IsNullOrEmpty(name))
            {
                offers = offers.Where(c => (c.Name.ToLower().Contains(name.ToLower())));
            }
            switch (order)
            {
                case Constants.nameAsc:
                    offers = offers.OrderBy(c => c.Name);
                    break;
                case Constants.priceAsc:
                    offers = offers.OrderBy(c => c.Price);
                    break;
                case Constants.checkDateAsc:
                    offers = offers.OrderBy(c => c.CheckDate);
                    break;
                case Constants.nameDesc:
                    offers = offers.OrderByDescending(c => c.Name);
                    break;
                case Constants.priceDesc:
                    offers = offers.OrderByDescending(c => c.Price);
                    break;
                case Constants.checkDateDesc:
                    offers = offers.OrderByDescending(c => c.CheckDate);
                    break;
                default:
                    offers = offers.OrderBy(c => c.CheckDate);
                    break;

            }
            var categoryMarketplace = _context.Marketplace.Find(category.MarketplaceId);
            var categoryVM = new CategoryViewModel(category, categoryMarketplace);
            var offersVM = new List<OfferViewModel>();
            foreach(var offer in offers)
            {
                offersVM.Add(new OfferViewModel(offer, categoryMarketplace, category));
            }
            var viewModel = new CategoryDetails(categoryVM, offersVM.ToPagedList(page, Constants.recordsPerPage));

            return View(viewModel);
        }
        [Authorize(Roles = Constants.administrator)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = _context.Category.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.administrator)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Category == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Category'  is null.");
            }
            var category = await _context.Category.FindAsync(id);
            if (category != null)
            {
                _context.Category.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return (_context.Category.Any(e => e.Id == id));
        }
    }
}
