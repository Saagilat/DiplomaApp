using DiplomaApp.Models;
using HtmlAgilityPack;
using DiplomaApp.Core;
using static DiplomaApp.WebScraper.Parser;

namespace DiplomaApp.WebScraper
{
    public class Crawler
    {
        public static async Task<List<Category>> GetCategories(Marketplace marketplace)
        {
            List<Category> result = new List<Category>();
            using var client = new HttpClient();
            var response = await client.GetAsync(marketplace.UrlCategories);
            Console.WriteLine(marketplace.UrlCategories);
            Console.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                var checkDate = DateTime.UtcNow;
                var responseContent = await response.Content.ReadAsStringAsync();
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(responseContent);
                var parsedCategories = ParseCategories(htmlDocument, marketplace);
                foreach(var category in parsedCategories)
                {
                    category.CheckDate = checkDate;
                    Console.WriteLine(category.Url);
                }
                result.AddRange(parsedCategories);
            }
            return result;
        }

        public class PageParseResult
        { 
            public List<Offer> Offers;
            public string NextPageUrl;
            public PageParseResult()
            {
               NextPageUrl = "";
               Offers = new List<Offer>();
            }
        }
        public static async Task<PageParseResult> ParseOffersPage(string pageUrl, Marketplace marketplace, int minWaitMilliseconds = Constants.minWaitMilliseconds, int maxWaitMilliseconds = Constants.maxWaitMilliseconds)
        {

            PageParseResult result = new PageParseResult();
            Random random = new Random();
            using var client = new HttpClient();
            var response = await client.GetAsync(pageUrl);

            Console.WriteLine(pageUrl + " >-> " + response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                var checkDate = DateTime.UtcNow;
                var responseContent = await response.Content.ReadAsStringAsync();
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(responseContent);
                var parsedOffers = ParseOffers(htmlDocument, marketplace);
                foreach (var offer in parsedOffers)
                {
                    offer.CheckDate = checkDate;
                }
                result.Offers.AddRange(parsedOffers);
                result.NextPageUrl = ParseNextUrl(htmlDocument, marketplace);
            }            

            Thread.Sleep(random.Next(minWaitMilliseconds, maxWaitMilliseconds));
            
            return result;
        }
    }
}
