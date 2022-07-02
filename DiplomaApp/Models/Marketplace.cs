using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomaApp.Models
{
    public class Marketplace
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public CatalogMap CatalogMap { get; set; }
        public CategoryMap CategoryMap { get; set; }
        public OfferMap OfferMap { get; set; }

        public ICollection<Category> Categories = new List<Category>();
        public Marketplace()
        {
        }
    }
    public class CatalogMap
    {
        public Marketplace Marketplace { get; set; }
        public int MarketplaceId { get; set; }
        public string UrlMarketplace { get; set; }
        public string Url { get; set; }
        public string XPathCategories { get; set; }
        public string XPathUrl { get; set; }
        public string XPathName { get; set; }
        public DateTime CheckDate { get; set; }
        public CatalogMap()
        {

        }
    }
    public class CategoryMap
    {
        public Marketplace Marketplace { get; set; }
        public int MarketplaceId { get; set; }
        public string UrlMarketplace { get; set; }
        public string XPathOffers { get; set; }
        public string XPathUrl { get; set; }
        public string XPathName { get; set; }
        public string XPathPrice { get; set; }
        public string XPathNextPageUrl { get; set; }
        public CategoryMap()
        {

        }
    }
    public class OfferMap
    {
        public Marketplace Marketplace { get; set; }
        public int MarketplaceId { get; set; }
        public string UrlMarketplace { get; set; }
        public string XPathName { get; set; }
        public string XPathPrice { get; set; }
        public OfferMap()
        {

        }
    }
}
