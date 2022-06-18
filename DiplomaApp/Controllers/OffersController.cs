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
            var offerCategory = _context.Category.Find(offer.CategoryId);
            var offerMarketplace = _context.Marketplace.Find(offerCategory.MarketplaceId);
            var offerPage = _context.OfferPage.Find(offerMarketplace.Id);
            if ((DateTime.UtcNow - offer.CheckDate).TotalMinutes > Constants.offerExpirationTimeMinutes)
            {
                var parsedOffer = await Crawler.ScrapeOfferPage(offer.Url, offerPage);
                
                if (!_context.OfferPrice.Any(c => c.OfferId == offer.Id && c.CheckDate == parsedOffer.CheckDate))
                {
                    offer.Price = parsedOffer.Price;
                    offer.Name = parsedOffer.Name;
                    offer.CheckDate = parsedOffer.CheckDate;
                    _context.OfferPrice.Add(new OfferPrice { Price = offer.Price, CheckDate = offer.CheckDate, Offer = offer, OfferId = offer.Id });
                }
                _context.SaveChanges();
            }
            var offers = Enumerable.AsEnumerable(_context.Offer).Where(c => c.Name.ToLower().Contains(offer.Name.ToLower()));
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
            if (offers.Any())
            {
                var offerPrices = new List<float>();
                foreach (var item in offers)
                {
                    offerPrices.AddRange(_context.OfferPrice.Where(c => c.OfferId == item.Id).Select(c => c.Price));
                }
                ViewData["MinPrice"] = offerPrices.Min();
                ViewData["MaxPrice"] = offerPrices.Max();
            }

            OfferDetails viewModel = new OfferDetails();
            viewModel.MarketplaceName = offerMarketplace.Name;
            viewModel.MarketplaceUrl = offerMarketplace.Url;
            viewModel.CategoryName = offerCategory.Name;
            viewModel.CategoryUrl = offerCategory.Url;
            viewModel.OfferName = offer.Name;
            viewModel.OfferUrl = offer.Url;
            List<Dataset> Datasets = new List<Dataset>();
            Random random = new Random();
            int iterator = 0;
            foreach(var item in offers)
            {
                var itemCategory = _context.Category.Find(item.CategoryId);
                var itemMarketplace = _context.Marketplace.Find(itemCategory.MarketplaceId);
                viewModel.SimilliarOffers.Add(new OfferIndex
                {
                    CategoryName = itemCategory.Name,
                    MarketplaceName = itemMarketplace.Name,
                    Offer = item
                });
                List<OfferPrice> offerPrices = _context.OfferPrice.Where(c => c.OfferId == item.Id).ToList();
                Dataset chartOfferDataset1 = new Dataset();
                int r = (iterator + 1) * (150 / offers.Count());
                int g = 150;
                int b = (iterator + 1) * (255 / offers.Count());
                chartOfferDataset1.label = itemMarketplace.Name + " - " + item.Name;
                chartOfferDataset1.borderColor = "rgb(" + r + ", " + g + ", " + b + ")";
                List<DataPoint> points1 = new List<DataPoint>();
                foreach(var offerPrice in offerPrices)
                {
                    points1.Add(new DataPoint() {x = offerPrice.CheckDate.ToLocalTime(), y = offerPrice.Price });
                }
                chartOfferDataset1.data = points1;
                Datasets.Add(chartOfferDataset1);
                viewModel.DatasetsJson = JsonConvert.SerializeObject(Datasets);
                viewModel.Datasets = Datasets;
                iterator++;
            }
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
