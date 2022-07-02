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
using DiplomaApp.Core;
using System.Web;
using X.PagedList;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using DiplomaApp.WebScraper;

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
            var offersVM = new List<OfferViewModel>();
            foreach(var offer in offers)
            {
                var offerCategory = _context.Category.Find(offer.CategoryId);
                var offerMarketplace = _context.Marketplace.Find(offerCategory.MarketplaceId);
                offersVM.Add(new OfferViewModel(offer, offerMarketplace, offerCategory));
            }
            var viewModel = new OfferIndex(offersVM.ToPagedList(page, Constants.recordsPerPage));
            return View(viewModel);            
        }

        [Authorize(Roles = Constants.administrator)]
        public async Task<IActionResult> UpdateOffers(List<Offer> offers)
        {
            foreach (var offer in offers)
            {
                var category = _context.Category.Find(offer.CategoryId);
                var OfferMap = _context.OfferMap.Find(category.MarketplaceId);
                if ((DateTime.UtcNow - offer.CheckDate).TotalMinutes > Constants.offerExpirationTimeMinutes)
                {
                    var parsedOffer = await Crawler.ScrapeOfferMap(offer.Url, OfferMap);
                    if (!String.IsNullOrEmpty(parsedOffer.Name) && parsedOffer.Price != null)
                    {
                        if (!_context.OfferPrice.Any(c => c.OfferId == offer.Id && c.CheckDate == parsedOffer.CheckDate))
                        {
                            offer.Price = parsedOffer.Price;
                            offer.Name = parsedOffer.Name;
                            offer.CheckDate = parsedOffer.CheckDate;
                            _context.OfferPrice.Add(new OfferPrice { Price = offer.Price, CheckDate = offer.CheckDate, Offer = offer, OfferId = offer.Id });
                        }
                    }
                }
            }
            await _context.SaveChangesAsync();
            return View();
        }

        // GET: Offers/Details/5
        public async Task<IActionResult> Details(int id)
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

            var similarOffers = _context.Offer.AsEnumerable().Where(c => c.Name.ToLower().Contains(offer.Name.ToLower()) || offer.Name.ToLower().Contains(c.Name.ToLower())).ToList();
            if (similarOffers.Any())
            {
                await UpdateOffers(similarOffers);
                var offerCategory = _context.Category.Find(offer.CategoryId);
                var offerMarketplace = _context.Marketplace.Find(offerCategory.MarketplaceId);
                var offerVM = new OfferViewModel(offer, offerMarketplace, offerCategory);
                var similarOffersVM = new List<OfferViewModel>();
                foreach(var item in similarOffers)
                {
                    var itemCategory = _context.Category.Find(item.CategoryId);
                    var itemMarketplace = _context.Marketplace.Find(itemCategory.MarketplaceId);
                    var itemPrices = _context.OfferPrice.Where(c => c.OfferId == item.Id).ToList();
                    similarOffersVM.Add(new OfferViewModel(item, itemMarketplace, itemCategory, itemPrices));
                }
                var viewModel = new OfferDetails(offerVM, similarOffersVM);

                return View(viewModel);
            }
            return View();
        }


        // GET: Offers/Delete/5
        [Authorize(Roles = Constants.administrator)]
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
        [Authorize(Roles = Constants.administrator)]
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
