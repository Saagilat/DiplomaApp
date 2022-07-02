using DiplomaApp.Models;
using HtmlAgilityPack;
using DiplomaApp.Core;
using static Useragents.Collection;
using static DiplomaApp.WebScraper.Parser;

namespace DiplomaApp.WebScraper
{
    public class Crawler
    {
        public class CatalogMapScrapeResult
        {
            public bool ResponseSuccesful;
            public List<Category> Categories;
            public CatalogMapScrapeResult()
            {
                ResponseSuccesful = false;
                Categories = new List<Category>();
            }
        }
        public static async Task<Offer> ScrapeOfferMap(string url, OfferMap OfferMap, int minWait = Constants.minWaitMilliseconds, int maxWait = Constants.maxWaitMilliseconds)
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
                    result = ParseOfferFromOfferMap(htmlDocument, OfferMap);
                    result.CheckDate = checkDate;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return result;
        }
        public static async Task<CatalogMapScrapeResult> ScrapeCatalogMap(CatalogMap CatalogMap, int minWait = Constants.minWaitMilliseconds, int maxWait = Constants.maxWaitMilliseconds)
        {
            CatalogMapScrapeResult result = new CatalogMapScrapeResult();
            Random random = new Random();
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(GetRandomDesktop());
            try
            {
                var response = await client.GetAsync(CatalogMap.Url);
                Console.WriteLine(CatalogMap.Url + ">---->" + response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    result.ResponseSuccesful = true;
                    var checkDate = DateTime.UtcNow;
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(responseContent);
                    var parsedCategories = ParseCatalogMap(htmlDocument, CatalogMap);
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
        
        public class CategoryMapScrapeResult
        {
            public bool ResponseSuccesful;
            public List<Offer> Offers;
            public string NextPageUrl;
            public CategoryMapScrapeResult()
            {
               ResponseSuccesful = false;
               NextPageUrl = "";
               Offers = new List<Offer>();
            }
        }
        public static async Task<CategoryMapScrapeResult> ScrapeCategoryMap(string url, CategoryMap CategoryMap, int minWait = Constants.minWaitMilliseconds, int maxWait = Constants.maxWaitMilliseconds)
        {

            CategoryMapScrapeResult result = new CategoryMapScrapeResult();
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
                    var parsedOffers = ParseCategoryMap(htmlDocument, CategoryMap);
                    foreach (var offer in parsedOffers)
                    {
                        offer.CheckDate = checkDate;
                    }
                    result.Offers.AddRange(parsedOffers);
                    result.NextPageUrl = ParseCategoryMapNextUrl(htmlDocument, CategoryMap);
                    Console.WriteLine("NextPageUrl = " + result.NextPageUrl);
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
