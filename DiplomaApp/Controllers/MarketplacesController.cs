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
    public class MarketplacesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MarketplacesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Marketplaces
        public async Task<IActionResult> Index(int page=1)
        {
            if (_context == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Marketplace'  is null.");
            }

            var marketplacesPage = _context.Marketplace.ToPagedList(page, Constants.recordsPerPage);
            return View(marketplacesPage);
        }
        // GET: Marketplaces/Details/5
        public async Task<IActionResult> Details(int id, int page=1)
        {
            if (id == null || _context.Marketplace == null)
            {
                return NotFound();
            }

            var marketplace = await _context.Marketplace
                .FirstOrDefaultAsync(m => m.Id == id);
            if (marketplace == null)
            {
                return NotFound();
            }
            MarketplaceDetails viewModel = new MarketplaceDetails();
            viewModel.Marketplace = marketplace;
            viewModel.Categories = _context.Category.Where(c => c.MarketplaceId == id).AsEnumerable().ToPagedList(page, Constants.recordsPerPage);

            return View(viewModel);

        }
        // GET: Marketplaces/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Marketplaces/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,UrlBase,UrlCategories,XPathCategories,XPathCategoryUrl,AttributeCategoryUrl,XPathCategoryName,AttributeCategoryName,XPathOffers,XPathOfferUrl,AttributeOfferUrl,XPathOfferName,AttributeOfferName,XPathOfferPrice,AttributeOfferPrice,XPathOfferPricePromotional,AttributeOfferPricePromotional,XPathNextPageUrl,AttributeNextPageUrl,OffersCheckDate,CategoriesCheckDate")] Marketplace marketplace)
        {
            if (ModelState.IsValid)
            {
                marketplace.CategoriesCheckDate = DateTime.UtcNow.AddDays(-7);
                _context.Add(marketplace);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(marketplace);
        }

        // GET: Marketplaces/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null || _context.Marketplace == null)
            {
                return NotFound();
            }

            var marketplace = await _context.Marketplace.FindAsync(id);
            if (marketplace == null)
            {
                return NotFound();
            }
            return View(marketplace);
        }

        // POST: Marketplaces/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,UrlBase,UrlCategories,XPathCategories,XPathCategoryUrl,AttributeCategoryUrl,XPathCategoryName,AttributeCategoryName,XPathOffers,XPathOfferUrl,AttributeOfferUrl,XPathOfferName,AttributeOfferName,XPathOfferPrice,AttributeOfferPrice,XPathOfferPricePromotional,AttributeOfferPricePromotional,XPathNextPageUrl,AttributeNextPageUrl,OffersCheckDate,CategoriesCheckDate")] Marketplace marketplace)
        {
            if (id != marketplace.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingMarketplace = _context.Marketplace.FirstOrDefault(c => c.Id == id);
                    existingMarketplace.Name = marketplace.Name;
                    existingMarketplace.UrlBase = marketplace.UrlBase;
                    existingMarketplace.UrlCategories = marketplace.UrlCategories;
                    existingMarketplace.AttributeCategoryName = marketplace.AttributeCategoryName;
                    existingMarketplace.AttributeCategoryUrl = marketplace.AttributeCategoryUrl;
                    existingMarketplace.AttributeOfferName = marketplace.AttributeOfferName;
                    existingMarketplace.AttributeOfferUrl = marketplace.AttributeOfferUrl;
                    existingMarketplace.AttributeOfferPrice = marketplace.AttributeOfferPrice;
                    existingMarketplace.AttributeOfferPricePromotional = marketplace.AttributeOfferPricePromotional;
                    existingMarketplace.AttributeNextPageUrl = marketplace.AttributeNextPageUrl;
                    existingMarketplace.XPathOffers = marketplace.XPathOffers;
                    existingMarketplace.XPathCategories = marketplace.XPathCategories;
                    existingMarketplace.XPathCategoryName = marketplace.XPathCategoryName;
                    existingMarketplace.XPathCategoryUrl = marketplace.XPathCategoryUrl;
                    existingMarketplace.XPathOfferName = marketplace.XPathOfferName;
                    existingMarketplace.XPathOfferUrl = marketplace.XPathOfferUrl;
                    existingMarketplace.XPathOfferPrice = marketplace.XPathOfferPrice;
                    existingMarketplace.XPathOfferPricePromotional = marketplace.XPathOfferPricePromotional;
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarketplaceExists(marketplace.Id))
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
            return View(marketplace);
        }

        // GET: Marketplaces/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null || _context.Marketplace == null)
            {
                return NotFound();
            }

            var marketplace = _context.Marketplace.First(m => m.Id == id);
            if (marketplace == null)
            {
                return NotFound();
            }

            return View(marketplace);
        }

        // POST: Marketplaces/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Marketplace == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Marketplace'  is null.");
            }
            var marketplace = await _context.Marketplace.FindAsync(id);
            if (marketplace != null)
            {
                _context.Marketplace.Remove(marketplace);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GetOffers(int id)
        {
            if (!MarketplaceExists(id))
            {
                return NotFound();
            }

            Marketplace marketplace = _context.Marketplace.FirstOrDefault(c => c.Id == id);
            DateTime startDateTime = DateTime.UtcNow;
            List<Category> parsedCategories = await Crawler.GetCategories(marketplace);

            if (parsedCategories != null)
            {
                foreach (var parsedCategory in parsedCategories)
                {

                    if (_context.Category.Any(e => e.Url == parsedCategory.Url))
                    {
                        var existingCategory = _context.Category.SingleOrDefault(c => c.Url == parsedCategory.Url);
                        existingCategory.Name = parsedCategory.Name;
                        existingCategory.CheckDate = parsedCategory.CheckDate;
                    }
                    else
                    {
                        parsedCategory.OffersCheckDate = DateTime.UtcNow.AddDays(-7);
                        parsedCategory.Marketplace = marketplace;
                        parsedCategory.MarketplaceId = marketplace.Id;
                        _context.Add(parsedCategory);
                    }
                }

                marketplace.CategoriesCheckDate = DateTime.UtcNow;
                _context.SaveChanges();
            }

            var categories = _context.Category.AsEnumerable()
                .Where(c => c.MarketplaceId == id && ((DateTime.UtcNow - c.OffersCheckDate).TotalHours > Constants.categoryGetOffersExpirationTimeHours))
                .OrderBy(c => c.OffersCheckDate);
            int iterator = 0;
            while((iterator < categories.Count()) && ((DateTime.UtcNow - startDateTime).TotalSeconds < Constants.parseTimeoutSeconds))
            {
                var category = categories.ElementAt(iterator);
                string pageUrl;
                if (String.IsNullOrEmpty(category.LastParsedPageUrl) || category.LastParsedPageUrl == marketplace.UrlBase)
                {
                    pageUrl = category.Url;
                }
                else
                {
                    pageUrl = category.LastParsedPageUrl;
                }
                while (pageUrl.Contains(category.Url) && ((DateTime.UtcNow - startDateTime).TotalSeconds < Constants.parseTimeoutSeconds))
                {
                    var parseResult = await Crawler.ParseOffersPage(pageUrl, marketplace, Constants.minWaitMilliseconds, Constants.maxWaitMilliseconds);
                    foreach (var parsedOffer in parseResult.Offers)
                    {
                        if (_context.Offer.Any(c => c.Url == parsedOffer.Url))
                        {
                            var existingOffer = _context.Offer.First(c => c.Url == parsedOffer.Url);
                            if (!_context.OfferPrice.Any(c => c.OfferId == existingOffer.Id && c.CheckDate == parsedOffer.CheckDate))
                            {
                                existingOffer.Name = parsedOffer.Name;
                                existingOffer.Price = parsedOffer.Price;
                                existingOffer.PricePromotional = parsedOffer.PricePromotional;
                                existingOffer.CheckDate = parsedOffer.CheckDate;
                                existingOffer.CategoryId = category.Id;
                                existingOffer.Category = category;
                                _context.OfferPrice.Add(new OfferPrice { Price = existingOffer.Price, PricePromotional = existingOffer.PricePromotional, CheckDate = existingOffer.CheckDate, Offer = existingOffer, OfferId = existingOffer.Id });
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
                if (!String.IsNullOrEmpty(category.LastParsedPageUrl))
                {
                    category.OffersCheckDate = DateTime.UtcNow;
                }
            }
            return RedirectToAction("Details", new { id = id });
        }

        public async Task<IActionResult> GetCategories(int id)
        {

            if (!MarketplaceExists(id))
            {
                return NotFound();
            }

            Marketplace marketplace = _context.Marketplace.Find(id);
            List<Category> parsedCategories = await Crawler.GetCategories(marketplace);

            if (parsedCategories != null)
            {
                foreach (var parsedCategory in parsedCategories)
                {
                    if (_context.Category.Any(e => e.Url == parsedCategory.Url))
                    {
                        var existingCategory = _context.Category.SingleOrDefault(c => c.Url == parsedCategory.Url);
                        existingCategory.Name = parsedCategory.Name;
                        existingCategory.CheckDate = parsedCategory.CheckDate;
                    }
                    else
                    {
                        parsedCategory.OffersCheckDate = DateTime.UtcNow.AddDays(-7);
                        parsedCategory.Marketplace = marketplace;
                        parsedCategory.MarketplaceId = marketplace.Id;
                        _context.Add(parsedCategory);
                    }
                }

                marketplace.CategoriesCheckDate = DateTime.UtcNow;
                _context.SaveChanges();
            }

            return RedirectToAction("Details", new { id = id });
        }
        public async Task<IActionResult> GetOffersAllMarketplaces()
        {
            var marketplaces = _context.Marketplace.AsEnumerable().Where(c => (DateTime.UtcNow - c.CategoriesCheckDate).TotalHours > Constants.marketplaceGetCategoriesExpirationTimeHours);
            DateTime startDateTime = DateTime.UtcNow;

            foreach (var marketplace in marketplaces)
            {
                var parsedCategories = await Crawler.GetCategories(marketplace);
                if (parsedCategories != null)
                {
                    foreach (var parsedCategory in parsedCategories)
                    {
                        if (_context.Category.Any(e => e.Url == parsedCategory.Url))
                        {
                            var existingCategory = _context.Category.SingleOrDefault(c => c.Url == parsedCategory.Url);
                            existingCategory.Name = parsedCategory.Name;
                            existingCategory.CheckDate = parsedCategory.CheckDate;
                        }
                        else
                        {
                            parsedCategory.OffersCheckDate = DateTime.UtcNow.AddDays(-7);
                            parsedCategory.Marketplace = marketplace;
                            parsedCategory.MarketplaceId = marketplace.Id;
                            _context.Add(parsedCategory);
                        }
                    }

                    marketplace.CategoriesCheckDate = DateTime.UtcNow;
                    _context.SaveChanges();
                }
            }

            var categories = _context.Category.AsEnumerable()
                .Where(c => (DateTime.UtcNow - c.OffersCheckDate).TotalHours > Constants.categoryGetOffersExpirationTimeHours)
                .OrderBy(c => c.OffersCheckDate);
            int iterator = 0;
            while ((iterator < categories.Count()) && ((DateTime.UtcNow - startDateTime).TotalSeconds < Constants.parseTimeoutSeconds))
            {
                var category = categories.ElementAt(iterator);
                var marketplace = _context.Marketplace.Find(category.MarketplaceId);
                string pageUrl;
                if (String.IsNullOrEmpty(category.LastParsedPageUrl) || category.LastParsedPageUrl == marketplace.UrlBase)
                {
                    pageUrl = category.Url;
                }
                else
                {
                    pageUrl = category.LastParsedPageUrl;
                }
                while (pageUrl.Contains(category.Url) && ((DateTime.UtcNow - startDateTime).TotalSeconds < Constants.parseTimeoutSeconds))
                {
                    var parseResult = await Crawler.ParseOffersPage(pageUrl, marketplace, Constants.minWaitMilliseconds, Constants.maxWaitMilliseconds);
                    foreach (var parsedOffer in parseResult.Offers)
                    {
                        if (_context.Offer.Any(c => c.Url == parsedOffer.Url))
                        {
                            var existingOffer = _context.Offer.First(c => c.Url == parsedOffer.Url);
                            if (!_context.OfferPrice.Any(c => c.OfferId == existingOffer.Id && c.CheckDate == parsedOffer.CheckDate))
                            {
                                existingOffer.Name = parsedOffer.Name;
                                existingOffer.Price = parsedOffer.Price;
                                existingOffer.PricePromotional = parsedOffer.PricePromotional;
                                existingOffer.CheckDate = parsedOffer.CheckDate;
                                existingOffer.CategoryId = category.Id;
                                existingOffer.Category = category;
                                _context.OfferPrice.Add(new OfferPrice { Price = existingOffer.Price, PricePromotional = existingOffer.PricePromotional, CheckDate = existingOffer.CheckDate, Offer = existingOffer, OfferId = existingOffer.Id });
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
                if (!String.IsNullOrEmpty(category.LastParsedPageUrl))
                {
                    category.OffersCheckDate = DateTime.UtcNow;
                }
            }
            return RedirectToAction("Index", new { });
        }
        private bool MarketplaceExists(int id)
        {
          return (_context.Marketplace.Any(e => e.Id == id));
        }
    }
}
