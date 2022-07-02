using System.ComponentModel.DataAnnotations;
using DiplomaApp.Models;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace DiplomaApp.ViewModels
{ 
    public class MarketplaceEdit
    {
        public MarketplaceViewModel Marketplace { get; set; }
        public CatalogMapViewModel CatalogMap { get; set; }
        public CategoryMapViewModel CategoryMap { get; set; }
        public OfferMapViewModel OfferMap { get; set; }

        public MarketplaceEdit(MarketplaceViewModel marketplace, CatalogMapViewModel catalogMap, CategoryMapViewModel categoryMap, OfferMapViewModel offerMap)
        {
            Marketplace = marketplace;
            CatalogMap = catalogMap;
            CategoryMap = categoryMap;
            OfferMap = offerMap;
        }
        public MarketplaceEdit()
        {

        }
    }
    public class MarketplaceIndex
    {
        public IPagedList<MarketplaceViewModel> Marketplaces { get; set; }
        public MarketplaceIndex(IPagedList<MarketplaceViewModel> marketplaces)
        {
            Marketplaces = marketplaces;
        }
    }
    public class MarketplaceDetails
    {
        public MarketplaceViewModel Marketplace { get; set; }
        public IPagedList<CategoryViewModel> Categories { get; set; }

        public MarketplaceDetails(MarketplaceViewModel marketplace, IPagedList<CategoryViewModel> categories)
        {
            Marketplace = marketplace;
            Categories = categories;
        }
    }
    public class MarketplaceDelete
    {
        public MarketplaceViewModel Marketplace { get; set; }
        public CatalogMapViewModel CatalogMap { get; set; }
        public CategoryMapViewModel CategoryMap { get; set; }
        public OfferMapViewModel OfferMap { get; set; }

        public MarketplaceDelete(MarketplaceViewModel marketplace, CatalogMapViewModel catalogMap, CategoryMapViewModel categoryMap, OfferMapViewModel offerMap)
        {
            Marketplace = marketplace;
            CatalogMap = catalogMap;
            CategoryMap = categoryMap;
            OfferMap = offerMap;
        }
    }
    public class MarketplaceViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "URL")]
        public string Url { get; set; }
        public MarketplaceViewModel(Marketplace marketplace)
        {
            Id = marketplace.Id;
            Name = marketplace.Name;
            Url = marketplace.Url;
        }
        public MarketplaceViewModel()
        {

        }
    }
    public class CatalogMapViewModel
    {
        [Display(Name = "URL")]
        public string Url { get; set; }
        public string XPathCategories { get; set; }
        public string XPathUrl { get; set; }
        public string XPathName { get; set; }
        public CatalogMapViewModel(CatalogMap catalogMap)
        {
            Url = catalogMap.Url;
            XPathCategories = catalogMap.XPathCategories;
            XPathUrl = catalogMap.XPathUrl;
            XPathName = catalogMap.XPathName;
        }
        public CatalogMapViewModel()
        {

        }
    }
    public class CategoryMapViewModel
    {
        public string XPathOffers { get; set; }
        public string XPathUrl { get; set; }
        public string XPathName { get; set; }
        public string XPathPrice { get; set; }
        public string XPathNextPageUrl { get; set; }
        public CategoryMapViewModel(CategoryMap categoryMap)
        {
            XPathOffers = categoryMap.XPathOffers;
            XPathUrl = categoryMap.XPathUrl;
            XPathName = categoryMap.XPathName;
            XPathPrice = categoryMap.XPathPrice;
            XPathNextPageUrl = categoryMap.XPathNextPageUrl;
        }
        public CategoryMapViewModel()
        {

        }
    }
    public class OfferMapViewModel
    {
        public string XPathName { get; set; }
        public string XPathPrice { get; set; }
        public OfferMapViewModel(OfferMap offerMap)
        {
            XPathName = offerMap.XPathName;
            XPathPrice = offerMap.XPathPrice;
        }
        public OfferMapViewModel()
        {

        }
    }
}
