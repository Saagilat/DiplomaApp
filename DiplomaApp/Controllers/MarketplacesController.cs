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
using static DiplomaApp.WebScraper.Crawler;

namespace DiplomaApp.Controllers
{
    public class MarketplacesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private static object GetOfferAllMarketplacesLock = new object();

        public MarketplacesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Marketplaces
        public async Task<IActionResult> Index(string name, int page=1)
        {
            if (_context == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Marketplace'  is null.");
            }
            var marketplaces = _context.Marketplace.AsEnumerable();
            if (!String.IsNullOrEmpty(name))
            {
                marketplaces = marketplaces.Where(c => (c.Name.ToLower().Contains(name.ToLower())));
            }
            List<MarketplaceViewModel> marketplacesVM = new List<MarketplaceViewModel>();
            foreach (var marketplace in marketplaces)
            {
                marketplacesVM.Add(new MarketplaceViewModel(marketplace));
            }
            var viewModel = new MarketplaceIndex(marketplacesVM.ToPagedList(page, Constants.recordsPerPage));
            return View(viewModel);
        }

        // GET: Marketplaces/Details/5
        public async Task<IActionResult> Details(int id, string name, int page=1)
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
            var categories = _context.Category.Where(c => c.MarketplaceId == id).AsEnumerable();
            if (!String.IsNullOrEmpty(name))
            {
                categories = categories.Where(c => c.Name.ToLower().Contains(name.ToLower()));
            }
            var marketplaceVM = new MarketplaceViewModel(marketplace);
            List<CategoryViewModel> categoriesVM = new List<CategoryViewModel>();
            foreach (var category in categories)
            {
                categoriesVM.Add(new CategoryViewModel(category, marketplace));
            }
            var viewModel = new MarketplaceDetails(marketplaceVM, categoriesVM.ToPagedList(page, Constants.recordsPerPage)); 
            return View(viewModel);

        }
        // GET: Marketplaces/Create
        [Authorize(Roles = Constants.administrator)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Marketplaces/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.administrator)]
        public async Task<IActionResult> Create(MarketplaceEdit marketplaceEdit)
        {

            if (ModelState.IsValid)
            {
                var marketplace = new Marketplace() { Name = marketplaceEdit.Marketplace.Name, Url = marketplaceEdit.Marketplace.Url };
                var CatalogMap = new CatalogMap()
                {
                    Url = marketplaceEdit.CatalogMap.Url,
                    UrlMarketplace = marketplaceEdit.Marketplace.Url,
                    XPathCategories = marketplaceEdit.CatalogMap.XPathCategories,
                    XPathUrl = marketplaceEdit.CatalogMap.XPathUrl,
                    XPathName = marketplaceEdit.CatalogMap.XPathName,
                    CheckDate = DateTime.UtcNow
                };
                var CategoryMap = new CategoryMap()
                {
                    UrlMarketplace = marketplaceEdit.Marketplace.Url,
                    XPathOffers = marketplaceEdit.CategoryMap.XPathOffers,
                    XPathUrl = marketplaceEdit.CategoryMap.XPathUrl,
                    XPathName = marketplaceEdit.CategoryMap.XPathName,
                    XPathPrice = marketplaceEdit.CategoryMap.XPathPrice,
                    XPathNextPageUrl = marketplaceEdit.CategoryMap.XPathNextPageUrl,
                };
                var OfferMap = new OfferMap()
                {
                    UrlMarketplace = marketplaceEdit.Marketplace.Url,
                    XPathName = marketplaceEdit.OfferMap.XPathName,
                    XPathPrice = marketplaceEdit.OfferMap.XPathPrice,
                };

                _context.Add(marketplace);
                await _context.SaveChangesAsync();
                CatalogMap.MarketplaceId = marketplace.Id;
                CategoryMap.MarketplaceId = marketplace.Id;
                OfferMap.MarketplaceId = marketplace.Id;
                _context.Add(CatalogMap);
                _context.Add(CategoryMap);
                _context.Add(OfferMap);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(marketplaceEdit);
        }

        // GET: Marketplaces/Edit/5
        [Authorize(Roles = Constants.administrator)]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null || _context.Marketplace == null)
            {
                return NotFound();
            }
            var marketplace = _context.Marketplace.FirstOrDefault(c => c.Id == id);

            if (marketplace == null)
            {
                return NotFound();
            }
            var marketplaceVM = new MarketplaceViewModel(marketplace);
            var catalogMapVM = new CatalogMapViewModel(_context.CatalogMap.FirstOrDefault(c => c.MarketplaceId == id));
            var categoryMapVM = new CategoryMapViewModel(_context.CategoryMap.FirstOrDefault(c => c.MarketplaceId == id));
            var offerMapVM = new OfferMapViewModel(_context.OfferMap.FirstOrDefault(c => c.MarketplaceId == id));

            MarketplaceEdit viewModel = new MarketplaceEdit(marketplaceVM, catalogMapVM, categoryMapVM, offerMapVM);
            return View(viewModel);
        }

        // POST: Marketplaces/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.administrator)]
        public async Task<IActionResult> Edit(int id, MarketplaceEdit marketplaceEdit)
        {
            if (ModelState.IsValid)
            {
                var marketplace = new Marketplace() { Id = id, Name = marketplaceEdit.Marketplace.Name, Url = marketplaceEdit.Marketplace.Url };
                var CatalogMap = new CatalogMap()
                {
                    MarketplaceId = id,
                    UrlMarketplace = marketplaceEdit.Marketplace.Url,
                    Url = marketplaceEdit.CatalogMap.Url,
                    XPathCategories = marketplaceEdit.CatalogMap.XPathCategories,
                    XPathUrl = marketplaceEdit.CatalogMap.XPathUrl,
                    XPathName = marketplaceEdit.CatalogMap.XPathName,
                    CheckDate = DateTime.UtcNow
                };
                var CategoryMap = new CategoryMap()
                {
                    MarketplaceId = id,
                    UrlMarketplace = marketplaceEdit.Marketplace.Url,
                    XPathOffers = marketplaceEdit.CategoryMap.XPathOffers,
                    XPathUrl = marketplaceEdit.CategoryMap.XPathUrl,
                    XPathName = marketplaceEdit.CategoryMap.XPathName,
                    XPathPrice = marketplaceEdit.CategoryMap.XPathPrice,
                    XPathNextPageUrl = marketplaceEdit.CategoryMap.XPathNextPageUrl,
                };
                var OfferMap = new OfferMap()
                {
                    MarketplaceId = id,
                    UrlMarketplace = marketplaceEdit.Marketplace.Url,
                    XPathName = marketplaceEdit.OfferMap.XPathName,
                    XPathPrice = marketplaceEdit.OfferMap.XPathPrice,
                };
                
                _context.Update(marketplace);
                _context.Update(CatalogMap);
                _context.Update(CategoryMap);
                _context.Update(OfferMap);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            
            return View(marketplaceEdit);
        }

        // GET: Marketplaces/Delete/5
        [Authorize(Roles = Constants.administrator)]
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
            var marketplaceVM = new MarketplaceViewModel(marketplace);
            var catalogMapVM = new CatalogMapViewModel(_context.CatalogMap.FirstOrDefault(c => c.MarketplaceId == id));
            var categoryMapVM = new CategoryMapViewModel(_context.CategoryMap.FirstOrDefault(c => c.MarketplaceId == id));
            var offerMapVM = new OfferMapViewModel(_context.OfferMap.FirstOrDefault(c => c.MarketplaceId == id));

            MarketplaceDelete viewModel = new MarketplaceDelete(marketplaceVM, catalogMapVM, categoryMapVM, offerMapVM);
            return View(viewModel);
        }

        // POST: Marketplaces/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.administrator)]
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

        [Authorize(Roles = Constants.administrator)]
        public async Task<IActionResult> GetCategoryOffers(int id, DateTime? startDateTime = null)
        {
            if (!_context.Category.Any(c => c.Id == id))
            {
                return NotFound();
            }

            if(startDateTime == null)
            {
                startDateTime = DateTime.UtcNow;
            }

            Category category = _context.Category.Find(id);
            Marketplace marketplace = _context.Marketplace.Find(category.MarketplaceId);
            var CategoryMap = _context.CategoryMap.Find(marketplace.Id);
            string pageUrl;

            Console.WriteLine("Category LastParsedPageUrl = " + category.LastParsedPageUrl);
            if ((String.IsNullOrEmpty(category.LastParsedPageUrl)) || !category.LastParsedPageUrl.Contains(category.Url))
            {
                pageUrl = category.Url;
                Console.WriteLine("Category Url = " + pageUrl);
            }
            else
            {
                pageUrl = category.LastParsedPageUrl;
                Console.WriteLine("Category LastParsedPageUrl = " + pageUrl);
            }
            while (!String.IsNullOrEmpty(pageUrl) && pageUrl.Contains(category.Url) && ((DateTime.UtcNow - startDateTime.Value).TotalSeconds < Constants.parseTimeoutSeconds))
            {
                Console.WriteLine(pageUrl);
                var scrapeResult = await ScrapeCategoryMap(pageUrl, CategoryMap, Constants.minWaitMilliseconds, Constants.maxWaitMilliseconds);
                foreach (var parsedOffer in scrapeResult.Offers)
                {
                    if (_context.Offer.Any(c => c.Url == parsedOffer.Url))
                    {
                        var existingOffer = _context.Offer.First(c => c.Url == parsedOffer.Url);
                        if (!_context.OfferPrice.Any(c => c.OfferId == existingOffer.Id && c.CheckDate == parsedOffer.CheckDate))
                        {
                            existingOffer.Name = parsedOffer.Name;
                            existingOffer.Price = parsedOffer.Price;
                            existingOffer.CheckDate = parsedOffer.CheckDate;
                            existingOffer.CategoryId = category.Id;
                            existingOffer.Category = category;
                            _context.OfferPrice.Add(new OfferPrice { Price = existingOffer.Price, CheckDate = existingOffer.CheckDate, Offer = existingOffer, OfferId = existingOffer.Id });
                        }
                    }
                    else
                    {
                        parsedOffer.CategoryId = category.Id;
                        parsedOffer.Category = category;
                        parsedOffer.CreationDate = parsedOffer.CheckDate;
                        _context.Add(parsedOffer);
                        if (!_context.OfferPrice.Any(c => c.OfferId == parsedOffer.Id && c.CheckDate == parsedOffer.CheckDate))
                        {
                            _context.OfferPrice.Add(new OfferPrice { Price = parsedOffer.Price, CheckDate = parsedOffer.CheckDate, Offer = parsedOffer, OfferId = parsedOffer.Id });
                        }
                    }
                }
                pageUrl = pageUrl == scrapeResult.NextPageUrl? "" : scrapeResult.NextPageUrl;
                category.LastParsedPageUrl = pageUrl;
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Categories", new { id = id });
        }
        [Authorize(Roles = Constants.administrator)]
        public async Task<IActionResult> GetOffers(int id, DateTime? startDateTime = null)
        {
            if (!MarketplaceExists(id))
            {
                return NotFound();
            }
            if (startDateTime == null)
            {
                startDateTime = DateTime.UtcNow;
            }

            Marketplace marketplace = _context.Marketplace.FirstOrDefault(c => c.Id == id);
            await GetCategories(id);

            var categories = _context.Category.AsEnumerable()
                .Where(c => c.MarketplaceId == id);
            int iterator = 0;
            CategoryMapScrapeResult scrapeResult = new CategoryMapScrapeResult();
            scrapeResult.ResponseSuccesful = true;
            while ((iterator < categories.Count()) && ((DateTime.UtcNow - startDateTime.Value).TotalSeconds < Constants.parseTimeoutSeconds) && scrapeResult.ResponseSuccesful)
            {
                var category = categories.ElementAt(iterator);
                await GetCategoryOffers(category.Id, startDateTime);
            }
            return RedirectToAction("Details", new { id = id });
        }
        [Authorize(Roles = Constants.administrator)]
        public async Task<IActionResult> GetCategories(int id)
        {

            if (!MarketplaceExists(id))
            {
                return NotFound();
            }

            var marketplace = _context.Marketplace.Find(id);
            var CatalogMap = _context.CatalogMap.Find(id);

            CatalogMapScrapeResult scrapeResult = await ScrapeCatalogMap(CatalogMap);

            if (scrapeResult.ResponseSuccesful && scrapeResult.Categories != null)
            {
                foreach (var parsedCategory in scrapeResult.Categories)
                {
                    if (_context.Category.Any(e => e.Url == parsedCategory.Url))
                    {
                        var existingCategory = _context.Category.SingleOrDefault(c => c.Url == parsedCategory.Url);
                        existingCategory.Name = parsedCategory.Name;
                        existingCategory.CheckDate = parsedCategory.CheckDate;
                    }
                    else
                    {
                        parsedCategory.CheckDate = DateTime.UtcNow;
                        parsedCategory.Marketplace = marketplace;
                        parsedCategory.MarketplaceId = id;
                        _context.Add(parsedCategory);
                    }
                }

                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return RedirectToAction("Details", new { id = id });
        }
        [Authorize(Roles = Constants.administrator)]
        public async Task<IActionResult> GetOffersAllMarketplaces()
        {

            var marketplaces = _context.Marketplace.Select(c => c.Id);
            DateTime startDateTime = DateTime.UtcNow;
            int iteratorMarketplacesId = marketplaces.Min();
            int maxMarketplacesId = marketplaces.Max();
            while (iteratorMarketplacesId <= maxMarketplacesId)
            {
                if (marketplaces.Contains(iteratorMarketplacesId))
                {
                    await GetCategories(iteratorMarketplacesId);
                }
                iteratorMarketplacesId++;
            }
            var categories = _context.Category.Select(c => c.Id);
            int iteratorCategoriesId = categories.Min();
            int maxCategoriesId = categories.Max();
            while ((iteratorCategoriesId <= maxCategoriesId) && (DateTime.UtcNow - startDateTime).TotalSeconds < Constants.parseTimeoutSeconds)
            {
                if (categories.Contains(iteratorCategoriesId))
                {
                    await GetCategoryOffers(iteratorCategoriesId);
                }
                iteratorCategoriesId++;
            }
            return RedirectToAction("Index", new { });
        }

        private bool MarketplaceExists(int id)
        {
          return (_context.Marketplace.Any(e => e.Id == id));
        }
    }
}
