using DiplomaApp.Models;
using HtmlAgilityPack;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;

namespace DiplomaApp.WebScraper
{
    public class Parser
    {
        public static Category ParseCategory(HtmlNode htmlNode, Marketplace marketplace)
        {
            var result = new Category();
            string url;
            string name;
            if (String.IsNullOrEmpty(marketplace.AttributeCategoryUrl))
            {
                url = htmlNode.SelectSingleNode(marketplace.XPathCategoryUrl).InnerText;
            }
            else
            {
                url = htmlNode.SelectSingleNode(marketplace.XPathCategoryUrl).GetAttributeValue(marketplace.AttributeCategoryUrl, "");
            }
            if (String.IsNullOrEmpty(marketplace.AttributeCategoryName))
            {
                name = htmlNode.SelectSingleNode(marketplace.XPathCategoryName).InnerText;
            }
            else
            {
                name = htmlNode.SelectSingleNode(marketplace.XPathCategoryName).GetAttributeValue(marketplace.AttributeCategoryName, "");
            }
            url = HttpUtility.HtmlDecode(url); 
            url = NormalizeUrl(url, marketplace);
            name = HttpUtility.HtmlDecode(name);
            name = name.Trim();
            result.Url = url;
            result.Name = name;
            return result;
        }
        public static List<Category> ParseCategories(HtmlDocument htmlDocument, Marketplace marketplace)
        {
            List<Category> result = new List<Category>();
            var categories = htmlDocument.DocumentNode.SelectNodes(marketplace.XPathCategories);
            if (categories != null)
            {
                foreach (var category in categories)
                {
                    var parsedCategory = ParseCategory(category, marketplace);
                    if (parsedCategory.Url != "")
                    {
                        result.Add(parsedCategory);
                    }
                }
            }
            return result;
        }
        public static Offer ParseOffer(HtmlNode htmlNode, Marketplace marketplace)
        {
            Offer result = new Offer();
            string url;
            string name;
            string priceString;
            string pricePromotionalString;
            float price;
            float pricePromotional;

            if (htmlNode != null)
            {
                var urlNode = htmlNode.SelectSingleNode(marketplace.XPathOfferUrl);
                var nameNode = htmlNode.SelectSingleNode(marketplace.XPathOfferName);
                var priceNode = htmlNode.SelectSingleNode(marketplace.XPathOfferPrice);
                var pricePromotionalNode = htmlNode.SelectSingleNode(marketplace.XPathOfferPricePromotional);
                if (urlNode == null)
                {
                    Console.WriteLine("URL NODE == NULL - XPathUrl probably incorrect");
                }
                if (nameNode == null)
                {
                    Console.WriteLine("NAME NODE == NULL - XPathName probably incorrect");
                }
                if(priceNode == null)
                {
                    Console.WriteLine("PRICE NODE == NULL - XPathPrice probably incorrect");
                }
                if (pricePromotionalNode == null)
                {
                    Console.WriteLine("PRICEPROMOTIONAL NODE == NULL - XPathPricePromotional probably incorrect");
                }
                if (urlNode != null && nameNode != null && priceNode != null && pricePromotionalNode != null)
                {
                    if (String.IsNullOrEmpty(marketplace.AttributeOfferUrl))
                    {
                        url = urlNode.InnerText;
                    }
                    else
                    {
                        url = urlNode.GetAttributeValue(marketplace.AttributeOfferUrl, "");
                    }
                    if (String.IsNullOrEmpty(marketplace.AttributeOfferName))
                    {
                        name = nameNode.InnerText;
                    }
                    else
                    {
                        name = nameNode.GetAttributeValue(marketplace.AttributeOfferName, "");
                    }
                    if (String.IsNullOrEmpty(marketplace.AttributeOfferPrice))
                    {
                        priceString = priceNode.InnerText;
                        priceString = Regex.Replace(priceString, "[^\\d.]", "");
                    }
                    else
                    {
                        priceString = priceNode.GetAttributeValue(marketplace.AttributeOfferPrice, "");
                        priceString = Regex.Replace(priceString, "[^\\d.]", ""); 
                    }
                    if (String.IsNullOrEmpty(marketplace.AttributeOfferPricePromotional))
                    {
                        pricePromotionalString = pricePromotionalNode.InnerText;
                        pricePromotionalString = Regex.Replace(pricePromotionalString, "[^\\d.]", "");
                    }
                    else
                    {
                        pricePromotionalString = pricePromotionalNode.GetAttributeValue(marketplace.AttributeOfferPricePromotional, "");
                        pricePromotionalString = Regex.Replace(pricePromotionalString, "[^\\d.]", "");
                    }
                    url = HttpUtility.HtmlDecode(url);
                    url = NormalizeUrl(url, marketplace);
                    name = HttpUtility.HtmlDecode(name);
                    name = name.Trim();
                    price = float.Parse(priceString, CultureInfo.InvariantCulture);
                    if (!String.IsNullOrEmpty(pricePromotionalString))
                    {
                        pricePromotional = float.Parse(pricePromotionalString, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        pricePromotional = price;
                    }
                    result.Url = url;
                    result.Name = name;
                    result.Price = price;
                    result.PricePromotional = pricePromotional;
                    Console.WriteLine(name + ": " + price + "|" + pricePromotional + " " + url);
                }
            }
            return result;
        }
        public static List<Offer> ParseOffers(HtmlDocument htmlDocument, Marketplace marketplace)
        {
            List<Offer> result = new List<Offer>();
            var offers = htmlDocument.DocumentNode.SelectNodes(marketplace.XPathOffers);
            if (offers != null)
            {
                foreach (var offer in offers)
                {
                    var parsedOffer = ParseOffer(offer, marketplace);
                    if (((parsedOffer != null && !String.IsNullOrEmpty(parsedOffer.Url)) && !String.IsNullOrEmpty(parsedOffer.Name)) && parsedOffer.Price != null)
                    {
                        result.Add(parsedOffer);
                    }
                }
            }
            else
            {
                Console.WriteLine("OFFERS == NULL - XPathCategory probably incorrect");
            }
            return result;
        }
        public static string ParseNextUrl(HtmlDocument htmlDocument, Marketplace marketplace)
        {
            string result = "";
            HtmlNode htmlNode = htmlDocument.DocumentNode.SelectSingleNode(marketplace.XPathNextPageUrl);
            if (htmlNode != null)
            {
                if (String.IsNullOrEmpty(marketplace.AttributeNextPageUrl))
                {
                    result = htmlNode.InnerText;
                }
                else
                {
                    result = htmlNode.GetAttributeValue(marketplace.AttributeNextPageUrl, "");
                }
            }
            result = HttpUtility.HtmlDecode(result);
            result = NormalizeUrl(result, marketplace);
            return result;
        }
        public static string NormalizeUrl(string url, Marketplace marketplace)
        {
            string result = url.Trim();
            if(!result.Contains(marketplace.UrlBase))
            {
                if (!(result.Contains("http") || (result.Contains("#"))))
                {
                    result = marketplace.UrlBase + url;
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
