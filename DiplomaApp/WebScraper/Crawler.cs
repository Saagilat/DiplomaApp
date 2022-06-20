using DiplomaApp.Models;
using HtmlAgilityPack;
using DiplomaApp.Core;
using static Useragents.Collection;
using static DiplomaApp.WebScraper.Parser;

namespace DiplomaApp.WebScraper
{
    public class Crawler
    {
        public class CatalogPageScrapeResult
        {
            public bool ResponseSuccesful;
            public List<Category> Categories;
            public CatalogPageScrapeResult()
            {
                ResponseSuccesful = false;
                Categories = new List<Category>();
            }
        }
        public static async Task<Offer> ScrapeOfferPage(string url, OfferPage offerPage, int minWait = Constants.minWaitMilliseconds, int maxWait = Constants.maxWaitMilliseconds)
        {
            Offer result = new Offer();
            Random random = new Random();
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(GetRandomDesktop());
            try
            {
                var response = await client.GetAsync(url);
                Console.WriteLine(url + ">---->" + response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    var checkDate = DateTime.UtcNow;
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(responseContent);
                    result = ParseOfferFromOfferPage(htmlDocument, offerPage);
                    result.CheckDate = checkDate;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return result;
        }
        public static async Task<CatalogPageScrapeResult> ScrapeCatalogPage(CatalogPage catalogPage, int minWait = Constants.minWaitMilliseconds, int maxWait = Constants.maxWaitMilliseconds)
        {
            CatalogPageScrapeResult result = new CatalogPageScrapeResult();
            Random random = new Random();
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(GetRandomDesktop());
            try
            {
                var response = await client.GetAsync(catalogPage.Url);
                Console.WriteLine(catalogPage.Url + ">---->" + response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    result.ResponseSuccesful = true;
                    var checkDate = DateTime.UtcNow;
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(responseContent);
                    var parsedCategories = ParseCatalogPage(htmlDocument, catalogPage);
                    foreach (var category in parsedCategories)
                    {
                        category.CheckDate = checkDate;
                        Console.WriteLine("    " + category.Url);
                    }
                    result.Categories.AddRange(parsedCategories);
                }
                Thread.Sleep(random.Next(minWait, maxWait));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return result;
        }
        
        public class CategoryPageScrapeResult
        {
            public bool ResponseSuccesful;
            public List<Offer> Offers;
            public string NextPageUrl;
            public CategoryPageScrapeResult()
            {
               ResponseSuccesful = false;
               NextPageUrl = "";
               Offers = new List<Offer>();
            }
        }
        public static async Task<CategoryPageScrapeResult> ScrapeCategoryPage(string url, CategoryPage categoryPage, int minWait = Constants.minWaitMilliseconds, int maxWait = Constants.maxWaitMilliseconds)
        {

            CategoryPageScrapeResult result = new CategoryPageScrapeResult();
            Random random = new Random();
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(GetRandomDesktop());
            
            try
            {
                var response = await client.GetAsync(url);
                Console.WriteLine(url + " >-> " + response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    result.ResponseSuccesful = true;
                    var checkDate = DateTime.UtcNow;
                    var responseContent = await response.Content.ReadAsStringAsync();
                    HtmlDocument htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(responseContent);
                    var parsedOffers = ParseCategoryPage(htmlDocument, categoryPage);
                    foreach (var offer in parsedOffers)
                    {
                        offer.CheckDate = checkDate;
                    }
                    result.Offers.AddRange(parsedOffers);
                    result.NextPageUrl = ParseCategoryPageNextUrl(htmlDocument, categoryPage);
                }
                Thread.Sleep(random.Next(minWait, maxWait));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return result;
        }
    }
}
