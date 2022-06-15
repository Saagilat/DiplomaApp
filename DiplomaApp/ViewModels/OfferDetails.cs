using DiplomaApp.Models;
using X.PagedList;

namespace DiplomaApp.ViewModels
{
    public class OfferDetails
    {
        public Offer Offer { get; set; }
        public string MarketplaceName { get; set; }
        public string MarketplaceUrl { get; set; }
        public string CategoryName { get; set; }
        public string CategoryUrl { get; set; }
        public IPagedList<OfferIndex> SimilliarOffers { get; set; }
        public List<ChartDataPoint> ChartPrices { get; set; }
        public List<ChartDataPoint> ChartPricesPromotional { get; set; }
        public OfferDetails()
        {

        }
    }
    public class ChartDataPoint
    {
        public DateTime x { get; set; }
        public double y { get; set; }
        public ChartDataPoint(DateTime x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
