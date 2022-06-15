using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomaApp.Models
{
    public class Marketplace
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UrlBase { get; set; }
        public string UrlCategories { get; set; }

        public string XPathCategories { get; set; }

        public string XPathCategoryUrl { get; set; }
        public string? AttributeCategoryUrl { get; set; }

        public string XPathCategoryName { get; set; }
        public string? AttributeCategoryName { get; set; }

        public string XPathOffers { get; set; }

        public string XPathOfferUrl { get; set; }
        public string? AttributeOfferUrl { get; set; }

        public string XPathOfferName { get; set; }
        public string? AttributeOfferName { get; set; }

        public string XPathOfferPrice { get; set; }
        public string? AttributeOfferPrice { get; set; }

        public string XPathOfferPricePromotional { get; set; }
        public string? AttributeOfferPricePromotional { get; set; }

        public string XPathNextPageUrl { get; set; }
        public string? AttributeNextPageUrl { get; set; }

        public DateTime CategoriesCheckDate { get; set; }

        public ICollection<Category> Categories = new List<Category>();
        public Marketplace()
        {
        }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public DateTime CheckDate { get; set; }
        public DateTime OffersCheckDate { get; set; }
        public string? LastParsedPageUrl { get; set; }
        public Marketplace Marketplace { get; set; }
        public int MarketplaceId { get; set; }

        public ICollection<Offer> Offers = new List<Offer>();
        public Category()
        {
        }
    }
    public class Offer
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public float PricePromotional { get; set; }
        public DateTime CheckDate { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }

        public ICollection<OfferPrice> OfferPrices = new List<OfferPrice>();

        public Offer()
        {
        }
    }

    public class OfferPrice
    {
        public float Price { get; set; }
        public float PricePromotional { get; set; }
        public DateTime CheckDate { get; set; }
        public Offer Offer { get; set; }
        public int OfferId { get; set; }


        public OfferPrice()
        {

        }
    }
}
