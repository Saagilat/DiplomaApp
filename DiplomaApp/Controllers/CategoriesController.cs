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
using FuzzySharp;
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
        public async Task<IActionResult> Index(int page=1)
        {

            if (_context == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Category'  is null.");
            }

            var categoriesPage = _context.Category.OrderBy(c => c.Name).ToPagedList(page, Constants.recordsPerPage);
            return View(categoriesPage);
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
            var viewModel = new CategoryDetails();
            viewModel.Category = category;
            var offers = _context.Offer.Where(c => c.CategoryId == id).AsEnumerable();
            if (offers.Any())
            {
                ViewData["MinPrice"] = offers.Min(c => c.Price);
                ViewData["MaxPrice"] = offers.Max(c => c.Price);
            }
            ViewData["Category"] = category.Name;
            ViewData["Marketplace"] = _context.Marketplace.Find(category.MarketplaceId).Name;
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
            List<OfferIndex> offerIndexes = new List<OfferIndex>();
            foreach (var item in offers)
            {
                offerIndexes.Add(new OfferIndex { Offer = item, CategoryName = _context.Category.Find(item.CategoryId).Name, MarketplaceName = _context.Marketplace.Find(_context.Category.Find(item.CategoryId).MarketplaceId).Name });
            }
            viewModel.Offers = offerIndexes.ToPagedList(page, Constants.recordsPerPage);

            return View(viewModel);
        }
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

        public async Task<IActionResult> GetOffers(int id)
        {

            if (!CategoryExists(id))
            {
                return NotFound();
            }

            Category category = _context.Category.Find(id);
            Marketplace marketplace = _context.Marketplace.Find(category.MarketplaceId);
            string pageUrl;
            if(String.IsNullOrEmpty(category.LastParsedPageUrl) || category.LastParsedPageUrl == marketplace.UrlBase)
            {
                pageUrl = category.Url;
            }
            else
            {
                pageUrl = category.LastParsedPageUrl;
            }
            DateTime startDateTime = DateTime.UtcNow;
            while(pageUrl.Contains(category.Url) && ((DateTime.UtcNow - startDateTime).TotalSeconds < Constants.parseTimeoutSeconds))
            {
                var parseResult = await Crawler.ParseOffersPage(pageUrl, marketplace, Constants.minWaitMilliseconds, Constants.maxWaitMilliseconds);
                foreach(var parsedOffer in parseResult.Offers)
                {
                    if(_context.Offer.Any(c => c.Url == parsedOffer.Url))
                    {
                        var existingOffer = _context.Offer.First(c => c.Url == parsedOffer.Url);
                        if(!_context.OfferPrice.Any(c => c.OfferId == existingOffer.Id && c.CheckDate == parsedOffer.CheckDate))
                        {
                            existingOffer.Name = parsedOffer.Name;
                            existingOffer.Price = parsedOffer.Price;
                            existingOffer.PricePromotional = parsedOffer.PricePromotional;
                            existingOffer.CheckDate = parsedOffer.CheckDate;
                            existingOffer.CategoryId = category.Id;
                            existingOffer.Category = category;
                            _context.OfferPrice.Add(new OfferPrice { Price = existingOffer.Price, PricePromotional = existingOffer.PricePromotional, CheckDate = existingOffer.CheckDate, Offer = existingOffer, OfferId = existingOffer.Id});
                        }
                    }
                    else
                    {
                        parsedOffer.CategoryId = category.Id;
                        parsedOffer.Category = category;
                        _context.Add(parsedOffer);
                        if (!_context.OfferPrice.Any(c => c.OfferId == parsedOffer.Id && c.CheckDate == parsedOffer.CheckDate))
                        {
                            _context.OfferPrice.Add(new OfferPrice { Price = parsedOffer.Price, PricePromotional = parsedOffer.PricePromotional, CheckDate = parsedOffer.CheckDate, Offer = parsedOffer, OfferId = parsedOffer.Id });
                        }
                    }
                }
                pageUrl = parseResult.NextPageUrl;
                category.LastParsedPageUrl = pageUrl;
                _context.SaveChanges();
            }
            if(!String.IsNullOrEmpty(category.LastParsedPageUrl))
            {
                category.OffersCheckDate = DateTime.UtcNow;
            }
            _context.SaveChanges();
            return RedirectToAction("Details", new { id = id });
        }

        private bool CategoryExists(int id)
        {
          return (_context.Category.Any(e => e.Id == id));
        }
    }
}
