using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DiplomaApp.Data;
using DiplomaApp.Models;
using DiplomaApp.ViewModels;
using FuzzySharp;
using DiplomaApp.Core;
using System.Web;
using X.PagedList;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace DiplomaApp.Controllers
{
    public class OffersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OffersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Offers
        public async Task<IActionResult> Index(string name, string category, string marketplace, int minPrice, int maxPrice, int page = 1, string order = "checkdate-desc", int similiarityRatio = Constants.defaultSimiliarityRatio)
        {
            if (_context == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Offer'  is null.");
            }
            var offers = _context.Offer.AsEnumerable();
            if (offers.Any())
            {
                ViewData["MinPrice"] = offers.Min(c => c.Price);
                ViewData["MaxPrice"] = offers.Max(c => c.Price);
            }
            if (maxPrice != 0)
            {
                offers = offers.Where(c => (c.Price <= maxPrice));
            }   
            if (minPrice != 0)
            {
                offers = offers.Where(c => (c.Price >= minPrice));
            }
            if(!String.IsNullOrEmpty(name))
            {
                offers = offers.Where(c => (c.Name.ToLower().Contains(name.ToLower())));
            }
            if(!String.IsNullOrEmpty(category))
            {
                offers = offers.Where(c => (_context.Category.Find(c.CategoryId).Name.ToLower().Contains(category.ToLower())));
            }
            if (!String.IsNullOrEmpty(marketplace))
            {
                offers = offers.Where(c => (_context.Marketplace.Find(_context.Category.Find(c.CategoryId).MarketplaceId).Name.ToLower().Contains(marketplace.ToLower())));
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
            foreach(var offer in offers)
            {
                offerIndexes.Add(new OfferIndex { Offer = offer, CategoryName = _context.Category.Find(offer.CategoryId).Name, MarketplaceName = _context.Marketplace.Find(_context.Category.Find(offer.CategoryId).MarketplaceId).Name });
            }
            IPagedList<OfferIndex> viewModel = offerIndexes.ToPagedList(page, Constants.recordsPerPage);
            return View(viewModel);            
        }

        // GET: Offers/Details/5
        public async Task<IActionResult> Details(int id, string category, string marketplace, int minPrice, int maxPrice, int page = 1, string order = "checkdate-desc", int similiarityRatio = Constants.defaultSimiliarityRatio)
        {
            if (id == null || _context.Offer == null)
            {
                return NotFound();
            }

            var offer = _context.Offer.Find(id);

            if (offer == null)
            {
                return NotFound();
            }
            var offers = Enumerable.AsEnumerable(_context.Offer).Where(c => c.Name.ToLower().Contains(offer.Name.ToLower()));
            if (offers.Any())
            {
                ViewData["MinPrice"] = offers.Min(c => c.Price);
                ViewData["MaxPrice"] = offers.Max(c => c.Price);
            }
            OfferDetails viewModel = new OfferDetails();
            ViewData["Name"] = offer.Name;
            if (maxPrice != 0)
            {
                offers = offers.Where(c => (c.Price <= maxPrice));
            }
            if (minPrice != 0)
            {
                offers = offers.Where(c => (c.Price >= minPrice));
            }
            if (!String.IsNullOrEmpty(category))
            {
                offers = offers.Where(c => (_context.Category.Find(c.CategoryId).Name.ToLower().Contains(category.ToLower())));
            }
            if (!String.IsNullOrEmpty(marketplace))
            {
                offers = offers.Where(c => (_context.Marketplace.Find(_context.Category.Find(c.CategoryId).MarketplaceId).Name.ToLower().Contains(marketplace.ToLower())));
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

            var offerIndexes = new List<OfferIndex>();
            foreach(var item in offers)
            {
                offerIndexes.Add(new OfferIndex { Offer = item, CategoryName = _context.Category.Find(item.CategoryId).Name, MarketplaceName = _context.Marketplace.Find(_context.Category.Find(item.CategoryId).MarketplaceId).Name });
            }
            List<ChartDataPoint> chartPrices = new List<ChartDataPoint>();
            List<ChartDataPoint> chartPricesPromotional = new List<ChartDataPoint>();

            var offerPrices = _context.OfferPrice.Where(c => c.OfferId == id);
            foreach (var item in offerPrices)
            {
                chartPrices.Add(new ChartDataPoint (item.CheckDate, item.Price));
                chartPricesPromotional.Add(new ChartDataPoint (item.CheckDate, item.PricePromotional));
            }

            var offerCategory = _context.Category.Find(offer.CategoryId);
            viewModel.Offer = offer;
            viewModel.CategoryName = offerCategory.Name;
            viewModel.CategoryUrl = offerCategory.Url;
            viewModel.MarketplaceName = _context.Marketplace.Find(offerCategory.MarketplaceId).Name;
            viewModel.MarketplaceUrl = _context.Marketplace.Find(offerCategory.MarketplaceId).UrlBase;
            viewModel.SimilliarOffers = offerIndexes.ToPagedList(page, Constants.recordsPerPage);
            viewModel.ChartPrices = chartPrices;
            viewModel.ChartPricesPromotional = chartPricesPromotional;
            return View(viewModel);
        }


        // GET: Offers/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null || _context.Offer == null)
            {
                return NotFound();
            }

            var offer = _context.Offer.Find(id);

            if (offer == null)
            {
                return NotFound();
            }

            return View(offer);
        }

        // POST: Offers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Offer == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Offer'  is null.");
            }
            var offer = _context.Offer.Find(id);
            if (offer != null)
            {
                _context.Offer.Remove(offer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool OfferExists(int id)
        {
          return _context.Offer.Any(e => e.Id == id);
        }
    }
}
