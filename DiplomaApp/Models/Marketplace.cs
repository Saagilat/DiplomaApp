using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomaApp.Models
{
    public class Marketplace
    {
        public int Id { get; set; }
        [Display(Name = "Название маркетплейса")]
        public string Name { get; set; }
        [Display(Name = "URL маркетплейса")]
        public string Url { get; set; }
        public CatalogPage CatalogPage { get; set; }
        public CategoryPage CategoryPage { get; set; }
        public OfferPage OfferPage { get; set; }

        public ICollection<Category> Categories = new List<Category>();
        public Marketplace()
        {
        }
    }
    public class CatalogPage
    {
        public Marketplace Marketplace { get; set; }
        public int MarketplaceId { get; set; }
        [Display(Name = "URL каталога")]
        public string UrlMarketplace { get; set; }
        public string Url { get; set; }
        [Display(Name = "XPathCategories каталога")]
        public string XPathCategories { get; set; }
        [Display(Name = "XPathUrl каталога")]
        public string XPathUrl { get; set; }
        [Display(Name = "AttributeUrl каталога")]
        public string AttributeUrl { get; set; }
        [Display(Name = "XPathName каталога")]
        public string XPathName { get; set; }
        public DateTime CheckDate { get; set; }
        public CatalogPage()
        {

        }
    }
    public class CategoryPage
    {
        public Marketplace Marketplace { get; set; }
        public int MarketplaceId { get; set; }
        [Display(Name = "URL категории")]
        public string UrlMarketplace { get; set; }
        [Display(Name = "XPathOffers категории")]
        public string XPathOffers { get; set; }
        [Display(Name = "XPathUrl категории")]
        public string XPathUrl { get; set; }
        [Display(Name = "AttributeUrl категории")]
        public string AttributeUrl { get; set; }
        [Display(Name = "XPathName категории")]
        public string XPathName { get; set; }
        [Display(Name = "XPathPrice категории")]
        public string XPathPrice { get; set; }
        [Display(Name = "XPathNextPageUrl категории")]
        public string XPathNextPageUrl { get; set; }
        [Display(Name = "AttributeNextPageUrl категории")]
        public string AttributeNextPageUrl { get; set; }
        public CategoryPage()
        {

        }
    }
    public class OfferPage
    {
        public Marketplace Marketplace { get; set; }
        public int MarketplaceId { get; set; }
        public string UrlMarketplace { get; set; }
        [Display(Name = "XPathName товара")]
        public string XPathName { get; set; }
        [Display(Name = "XPathPrice товара")]
        public string XPathPrice { get; set; }
        public OfferPage()
        {

        }
    }
}
