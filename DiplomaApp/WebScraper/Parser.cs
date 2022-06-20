using DiplomaApp.Models;
using HtmlAgilityPack;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;

namespace DiplomaApp.WebScraper
{
    public class Parser
    {
        public static List<Category> ParseCatalogPage(HtmlDocument htmlDocument, CatalogPage catalogPage)
        {
            List<Category> result = new List<Category>();
            var categoryNodes = htmlDocument.DocumentNode.SelectNodes(catalogPage.XPathCategories);
            if (categoryNodes != null)
            {
                foreach (var categoryNode in categoryNodes)
                {
                    var parsedCategory = new Category();
                    string url;
                    string name;
                    url = categoryNode.SelectSingleNode(catalogPage.XPathUrl).GetAttributeValue(catalogPage.AttributeUrl, "");
                    name = categoryNode.SelectSingleNode(catalogPage.XPathName).InnerText;
                    url = HttpUtility.HtmlDecode(url);
                    url = NormalizeUrl(url, catalogPage.UrlMarketplace);
                    name = name.Trim();
                    parsedCategory.Url = url;
                    parsedCategory.Name = name;
                    if (!String.IsNullOrEmpty(parsedCategory.Url))
                    {
                        result.Add(parsedCategory);
                    }
                }
            }
            else
            {
                Console.WriteLine("CATEGORY NODES == NULL XPathCategories probably incorrect");
                Console.WriteLine(catalogPage.XPathCategories);
            }
            return result;
        }

        public static List<Offer> ParseCategoryPage(HtmlDocument htmlDocument, CategoryPage categoryPage)
        {
            List<Offer> result = new List<Offer>();
            var offerNodes = htmlDocument.DocumentNode.SelectNodes(categoryPage.XPathOffers);
            if (offerNodes != null)
            {
                foreach (var offerNode in offerNodes)
                {
                    Offer parsedOffer = new Offer();
                    
                    if (offerNode != null)
                    {
                        string url;
                        string name;
                        string priceString;
                        float price;
                        var urlNode = offerNode.SelectSingleNode(categoryPage.XPathUrl);
                        var nameNode = offerNode.SelectSingleNode(categoryPage.XPathName);
                        var priceNode = offerNode.SelectSingleNode(categoryPage.XPathPrice);
                        if (urlNode == null)
                        {
                            Console.WriteLine("URL NODE == NULL - XPathUrl probably incorrect");
                            Console.WriteLine(categoryPage.XPathUrl);
                        }
                        if (nameNode == null)
                        {
                            Console.WriteLine("NAME NODE == NULL - XPathName probably incorrect");
                            Console.WriteLine(categoryPage.XPathName);
                        }
                        if (priceNode == null)
                        {
                            Console.WriteLine("PRICE NODE == NULL - XPathPrice probably incorrect");
                            Console.WriteLine(categoryPage.XPathPrice);
                        }
                        if (urlNode != null && nameNode != null && priceNode != null)
                        {
                            url = urlNode.GetAttributeValue(categoryPage.AttributeUrl, "");
                            name = nameNode.InnerText;
                            priceString = priceNode.InnerText;
                            priceString = HttpUtility.HtmlDecode(priceString);
                            priceString = Regex.Replace(priceString, "[^\\d.]", "");
                            url = HttpUtility.HtmlDecode(url);
                            url = NormalizeUrl(url, categoryPage.UrlMarketplace);
                            name = HttpUtility.HtmlDecode(name);
                            name = name.Trim();
                            price = float.Parse(priceString, CultureInfo.InvariantCulture);
                            parsedOffer.Url = url;
                            parsedOffer.Name = name;
                            parsedOffer.Price = price;
                            Console.WriteLine(name + ": " + price + " " + url);
                            if (((parsedOffer != null && !String.IsNullOrEmpty(parsedOffer.Url)) && !String.IsNullOrEmpty(parsedOffer.Name)) && parsedOffer.Price != null)
                            {
                                result.Add(parsedOffer);
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("OFFERS NODE == NULL - XPathCategory probably incorrect");
                Console.WriteLine(categoryPage.XPathOffers);
            }
            return result;
        }
        public static string ParseCategoryPageNextUrl(HtmlDocument htmlDocument, CategoryPage categoryPage)
        {
            string result = "";
            HtmlNode htmlNode = htmlDocument.DocumentNode.SelectSingleNode(categoryPage.XPathNextPageUrl);
            if (htmlNode != null)
            {
                result = htmlNode.GetAttributeValue(categoryPage.AttributeNextPageUrl, "");
            }
            else
            {
                Console.WriteLine("NEXT PAGE URL NODE == NULL - end of category, or categoryXPathNextPageUrl incorrect");
                Console.WriteLine(categoryPage.XPathNextPageUrl);
            }
            result = HttpUtility.HtmlDecode(result);
            result = NormalizeUrl(result, categoryPage.UrlMarketplace);
            return result;
        }

        public static Offer ParseOfferFromOfferPage(HtmlDocument htmlDocument, OfferPage offerPage)
        {
            Offer result = new Offer();
            var nameNode = htmlDocument.DocumentNode.SelectSingleNode(offerPage.XPathName);
            var priceNode = htmlDocument.DocumentNode.SelectSingleNode(offerPage.XPathPrice);
            string name;
            string priceString;
            float price;
            if (nameNode == null)
            {
                Console.WriteLine("OFFER PAGE NAME NODE == NULL - XPathName probably incorrect");
                Console.WriteLine(offerPage.XPathName);
            }
            if (priceNode == null)
            {
                Console.WriteLine("OFFER PAGE PRICE NODE == NULL - XPathPrice probably incorrect");
                Console.WriteLine(offerPage.XPathPrice);
            }
            if (nameNode != null && priceNode != null)
            {
                name = nameNode.InnerText;
                name = HttpUtility.HtmlDecode(name);
                name = name.Trim();
                priceString = priceNode.InnerText;
                priceString = HttpUtility.HtmlDecode(priceString);
                priceString = Regex.Replace(priceString, "[^\\d.]", "");
                price = float.Parse(priceString, CultureInfo.InvariantCulture);
                result.Price = price;
                result.Name = name;
                Console.WriteLine(name + ": " + price);
            }
            return result;
        }

        public static string NormalizeUrl(string url, string rootUrl)
        {
            string result = url.Trim();
            if(!result.Contains(rootUrl))
            {
                if (!(result.Contains("http") || (result.Contains("#"))))
                {
                    result = rootUrl + url;
                }
                else
                {
                    result = "";
                }
            }
            return result;
        }
    }
}
