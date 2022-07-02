using DiplomaApp.Models;
using HtmlAgilityPack;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;

namespace DiplomaApp.WebScraper
{
    public class Parser
    {
        public static List<Category> ParseCatalogMap(HtmlDocument htmlDocument, CatalogMap CatalogMap)
        {
            List<Category> result = new List<Category>();
            var categoryNodes = htmlDocument.DocumentNode.SelectNodes(CatalogMap.XPathCategories);
            if (categoryNodes != null)
            {
                foreach (var categoryNode in categoryNodes)
                {
                    var parsedCategory = new Category();
                    string url;
                    string name;
                    url = categoryNode.SelectSingleNode(CatalogMap.XPathUrl).GetAttributeValue("href", "");
                    name = categoryNode.SelectSingleNode(CatalogMap.XPathName).InnerText;
                    url = HttpUtility.HtmlDecode(url);
                    url = NormalizeUrl(url, CatalogMap.UrlMarketplace);
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
                Console.WriteLine(CatalogMap.XPathCategories);
            }
            return result;
        }

        public static List<Offer> ParseCategoryMap(HtmlDocument htmlDocument, CategoryMap CategoryMap)
        {
            List<Offer> result = new List<Offer>();
            var offerNodes = htmlDocument.DocumentNode.SelectNodes(CategoryMap.XPathOffers);
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
                        var urlNode = offerNode.SelectSingleNode(CategoryMap.XPathUrl);
                        var nameNode = offerNode.SelectSingleNode(CategoryMap.XPathName);
                        var priceNode = offerNode.SelectSingleNode(CategoryMap.XPathPrice);
                        if (urlNode == null)
                        {
                            Console.WriteLine("URL NODE == NULL - XPathUrl probably incorrect");
                            Console.WriteLine(CategoryMap.XPathUrl);
                        }
                        if (nameNode == null)
                        {
                            Console.WriteLine("NAME NODE == NULL - XPathName probably incorrect");
                            Console.WriteLine(CategoryMap.XPathName);
                        }
                        if (priceNode == null)
                        {
                            Console.WriteLine("PRICE NODE == NULL - XPathPrice probably incorrect");
                            Console.WriteLine(CategoryMap.XPathPrice);
                        }
                        if (urlNode != null && nameNode != null && priceNode != null)
                        {
                            url = urlNode.GetAttributeValue("href", "");
                            name = nameNode.InnerText;
                            priceString = priceNode.InnerText;
                            priceString = HttpUtility.HtmlDecode(priceString);
                            priceString = Regex.Replace(priceString, "[^\\d.]", "");
                            url = HttpUtility.HtmlDecode(url);
                            url = NormalizeUrl(url, CategoryMap.UrlMarketplace);
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
                Console.WriteLine(CategoryMap.XPathOffers);
            }
            return result;
        }
        public static string ParseCategoryMapNextUrl(HtmlDocument htmlDocument, CategoryMap CategoryMap)
        {
            string result = "";
            HtmlNode htmlNode = htmlDocument.DocumentNode.SelectSingleNode(CategoryMap.XPathNextPageUrl);
            if (htmlNode != null)
            {
                result = htmlNode.GetAttributeValue("href", "");
            }
            else
            {
                Console.WriteLine("NEXT PAGE URL NODE == NULL - end of category, or categoryXPathNextPageUrl incorrect");
                Console.WriteLine(CategoryMap.XPathNextPageUrl);
            }
            result = HttpUtility.HtmlDecode(result);
            result = NormalizeUrl(result, CategoryMap.UrlMarketplace);
            return result;
        }

        public static Offer ParseOfferFromOfferMap(HtmlDocument htmlDocument, OfferMap OfferMap)
        {
            Offer result = new Offer();
            var nameNode = htmlDocument.DocumentNode.SelectSingleNode(OfferMap.XPathName);
            var priceNode = htmlDocument.DocumentNode.SelectSingleNode(OfferMap.XPathPrice);
            string name;
            string priceString;
            float price;
            if (nameNode == null)
            {
                Console.WriteLine("OFFER PAGE NAME NODE == NULL - XPathName probably incorrect");
                Console.WriteLine(OfferMap.XPathName);
            }
            if (priceNode == null)
            {
                Console.WriteLine("OFFER PAGE PRICE NODE == NULL - XPathPrice probably incorrect");
                Console.WriteLine(OfferMap.XPathPrice);
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
