using DiplomaApp.Models;

namespace DiplomaApp.ViewModels
{
    public class MarketplaceEdit
    {
        public string MarketplaceName { get; set; }
        public string MarketplaceUrl { get; set; }
        public string CatalogPageUrl{ get; set; }
        public string CatalogPageXPathCategories{ get; set; }
        public string CatalogPageXPathUrl{ get; set; }
        public string CatalogPageAttributeUrl{ get; set; }
        public string CatalogPageXPathName{ get; set; }
        public string CategoryPageXPathOffers{ get; set; }
        public string CategoryPageXPathUrl{ get; set; }
        public string CategoryPageAttributeUrl{ get; set; }
        public string CategoryPageXPathName{ get; set; }
        public string CategoryPageXPathPrice{ get; set; }
        public string CategoryPageXPathNextPageUrl{ get; set; }
        public string CategoryPageAttributeNextPageUrl{ get; set; }
        public string OfferPageXPathName{ get; set; }
        public string OfferPageXPathPrice{ get; set; }

        public MarketplaceEdit()
        {

        }
    }
}
