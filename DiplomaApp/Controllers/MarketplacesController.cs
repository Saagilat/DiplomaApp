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

            var marketplacesPage = marketplaces.ToPagedList(page, Constants.recordsPerPage);
            return View(marketplacesPage);
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
            MarketplaceDetails viewModel = new MarketplaceDetails();
            ViewData["Marketplace"] = marketplace.Name;

            viewModel.Marketplace = marketplace;
            var categories = _context.Category.Where(c => c.MarketplaceId == id).AsEnumerable();
            if (!String.IsNullOrEmpty(name))
            {
                categories = categories.Where(c => c.Name.ToLower().Contains(name.ToLower()));
            }

            List<CategoryIndex> categoryIndexes = new List<CategoryIndex>();
            foreach (var category in categories)
            {;
                categoryIndexes.Add(new CategoryIndex() { Category = category, MarketplaceName = marketplace.Name, MarketplaceUrl = marketplace.Url });
            }
            viewModel.Categories = categoryIndexes.ToPagedList(page, Constants.recordsPerPage);

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
        public async Task<IActionResult> Create(MarketplaceEdit marketplaceEdit)
        {

            if (ModelState.IsValid)
            {
                var marketplace = new Marketplace() { Name = marketplaceEdit.MarketplaceName, Url = marketplaceEdit.MarketplaceUrl };
                var catalogPage = new CatalogPage()
                {
                    Url = marketplaceEdit.CatalogPageUrl,
                    UrlMarketplace = marketplaceEdit.MarketplaceUrl,
                    XPathCategories = marketplaceEdit.CatalogPageXPathCategories,
                    XPathUrl = marketplaceEdit.CatalogPageXPathUrl,
                    AttributeUrl = marketplaceEdit.CatalogPageAttributeUrl,
                    XPathName = marketplaceEdit.CatalogPageXPathName,
                    CheckDate = DateTime.UtcNow
                };
                var categoryPage = new CategoryPage()
                {
                    UrlMarketplace = marketplaceEdit.MarketplaceUrl,
                    XPathOffers = marketplaceEdit.CategoryPageXPathOffers,
                    XPathUrl = marketplaceEdit.CategoryPageXPathUrl,
                    AttributeUrl = marketplaceEdit.CategoryPageAttributeUrl,
                    XPathName = marketplaceEdit.CategoryPageXPathName,
                    XPathPrice = marketplaceEdit.CategoryPageXPathPrice,
                    XPathNextPageUrl = marketplaceEdit.CategoryPageXPathNextPageUrl,
                    AttributeNextPageUrl = marketplaceEdit.CategoryPageAttributeNextPageUrl,
                };
                var offerPage = new OfferPage()
                {
                    UrlMarketplace = marketplaceEdit.MarketplaceUrl,
                    XPathName = marketplaceEdit.OfferPageXPathName,
                    XPathPrice = marketplaceEdit.OfferPageXPathPrice,
                };

                _context.Add(marketplace);
                await _context.SaveChangesAsync();
                catalogPage.MarketplaceId = marketplace.Id;
                categoryPage.MarketplaceId = marketplace.Id;
                offerPage.MarketplaceId = marketplace.Id;
                _context.Add(catalogPage);
                _context.Add(categoryPage);
                _context.Add(offerPage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(marketplaceEdit);
        }

        // GET: Marketplaces/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null || _context.Marketplace == null)
            {
                return NotFound();
            }
            var marketplace = _context.Marketplace.FirstOrDefault(c => c.Id == id);
            var catalogPage = _context.CatalogPage.FirstOrDefault(c => c.MarketplaceId == id);
            var categoryPage = _context.CategoryPage.FirstOrDefault(c => c.MarketplaceId == id);
            var offerPage = _context.OfferPage.FirstOrDefault(c => c.MarketplaceId == id);

            if (marketplace == null)
            {
                return NotFound();
            }
            MarketplaceEdit viewModel = new MarketplaceEdit();
            viewModel.MarketplaceUrl = marketplace.Url;
            viewModel.MarketplaceName = marketplace.Name;
            if (catalogPage != null)
            {
                viewModel.CatalogPageUrl = catalogPage.Url;
                viewModel.CatalogPageXPathCategories = catalogPage.XPathCategories;
                viewModel.CatalogPageXPathUrl = catalogPage.XPathUrl;
                viewModel.CatalogPageAttributeUrl = catalogPage.AttributeUrl;
                viewModel.CatalogPageXPathName = catalogPage.XPathName;
            }
            if (categoryPage != null)
            {
                viewModel.CategoryPageXPathOffers = categoryPage.XPathOffers;
                viewModel.CategoryPageXPathUrl = categoryPage.XPathUrl;
                viewModel.CategoryPageAttributeUrl = categoryPage.AttributeUrl;
                viewModel.CategoryPageXPathName = categoryPage.XPathName;
                viewModel.CategoryPageXPathPrice = categoryPage.XPathPrice;
                viewModel.CategoryPageXPathNextPageUrl = categoryPage.XPathNextPageUrl;
                viewModel.CategoryPageAttributeNextPageUrl = categoryPage.AttributeNextPageUrl;
            }
            if (offerPage != null)
            {
                viewModel.OfferPageXPathName = offerPage.XPathName;
                viewModel.OfferPageXPathPrice = offerPage.XPathPrice;
            }
            return View(viewModel);
        }

        // POST: Marketplaces/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MarketplaceEdit marketplaceEdit)
        {
            if (ModelState.IsValid)
            {
                var marketplace = new Marketplace() { Id = id, Name = marketplaceEdit.MarketplaceName, Url = marketplaceEdit.MarketplaceUrl };
                var catalogPage = new CatalogPage()
                {
                    MarketplaceId = id,
                    UrlMarketplace = marketplaceEdit.MarketplaceUrl,
                    Url = marketplaceEdit.CatalogPageUrl,
                    XPathCategories = marketplaceEdit.CatalogPageXPathCategories,
                    XPathUrl = marketplaceEdit.CatalogPageXPathUrl,
                    AttributeUrl = marketplaceEdit.CatalogPageAttributeUrl,
                    XPathName = marketplaceEdit.CatalogPageXPathName,
                    CheckDate = DateTime.UtcNow
                };
                var categoryPage = new CategoryPage()
                {
                    MarketplaceId = id,
                    UrlMarketplace = marketplaceEdit.MarketplaceUrl,
                    XPathOffers = marketplaceEdit.CategoryPageXPathOffers,
                    XPathUrl = marketplaceEdit.CategoryPageXPathUrl,
                    AttributeUrl = marketplaceEdit.CategoryPageAttributeUrl,
                    XPathName = marketplaceEdit.CategoryPageXPathName,
                    XPathPrice = marketplaceEdit.CategoryPageXPathPrice,
                    XPathNextPageUrl = marketplaceEdit.CategoryPageXPathNextPageUrl,
                    AttributeNextPageUrl = marketplaceEdit.CategoryPageAttributeNextPageUrl,
                };
                var offerPage = new OfferPage()
                {
                    MarketplaceId = id,
                    UrlMarketplace = marketplaceEdit.MarketplaceUrl,
                    XPathName = marketplaceEdit.OfferPageXPathName,
                    XPathPrice = marketplaceEdit.OfferPageXPathPrice,
                };
                
                _context.Update(marketplace);
                _context.Update(catalogPage);
                _context.Update(categoryPage);
                _context.Update(offerPage);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            
            return View(marketplaceEdit);
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
            var categoryPage = _context.CategoryPage.Find(marketplace.Id);
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
                var scrapeResult = await ScrapeCategoryPage(pageUrl, categoryPage, Constants.minWaitMilliseconds, Constants.maxWaitMilliseconds);
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
                        _context.Add(parsedOffer);
                        if (!_context.OfferPrice.Any(c => c.OfferId == parsedOffer.Id && c.CheckDate == parsedOffer.CheckDate))
                        {
                            _context.OfferPrice.Add(new OfferPrice { Price = parsedOffer.Price, CheckDate = parsedOffer.CheckDate, Offer = parsedOffer, OfferId = parsedOffer.Id });
                        }
                    }
                }
                pageUrl = scrapeResult.NextPageUrl;
                category.LastParsedPageUrl = pageUrl;
                _context.SaveChanges();
            }

            _context.SaveChanges();

            return RedirectToAction("Details", "Categories", new { id = id });
        }
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
            CategoryPageScrapeResult scrapeResult = new CategoryPageScrapeResult();
            scrapeResult.ResponseSuccesful = true;
            while ((iterator < categories.Count()) && ((DateTime.UtcNow - startDateTime.Value).TotalSeconds < Constants.parseTimeoutSeconds) && scrapeResult.ResponseSuccesful)
            {
                var category = categories.ElementAt(iterator);
                await GetCategoryOffers(category.Id, startDateTime);
            }
            return RedirectToAction("Details", new { id = id });
        }

        public async Task<IActionResult> GetCategories(int id)
        {

            if (!MarketplaceExists(id))
            {
                return NotFound();
            }

            var marketplace = _context.Marketplace.Find(id);
            var catalogPage = _context.CatalogPage.Find(id);

            CatalogPageScrapeResult scrapeResult = await Crawler.ScrapeCatalogPage(catalogPage);

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

                _context.SaveChanges();
            }

            return RedirectToAction("Details", new { id = id });
        }
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
