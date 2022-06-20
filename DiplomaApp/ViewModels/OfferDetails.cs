using DiplomaApp.Models;
using X.PagedList;

namespace DiplomaApp.ViewModels
{
    public class OfferDetails
    {

        public string MarketplaceName { get; set; }
        public string MarketplaceUrl { get; set; }
        public string CategoryName { get; set; }
        public string CategoryUrl { get; set; }
        public string OfferName { get; set; }
        public string OfferUrl { get; set; }
        public DateTime OfferCreationDate { get; set; }

        public List<OfferIndex> SimilliarOffers = new List<OfferIndex>();
        public string DatasetsJson { get; set; }
        public List<Dataset> Datasets { get; set; }
        public OfferDetails()
        {

        }
    }
    public class Dataset
    {
        public string label { get; set; }

        public List<DataPoint> data = new List<DataPoint>();
        public string borderColor {get; set;}
        public Dictionary<string, string> trendlineLinear = new Dictionary<string, string>
        {
            {"projection:", "true"},
            {"lineStyle", "dotted" },
            {"width:", "1" }
        };

        public Dataset()
        {

        }
    }
    public class DataPoint
    {
        public DateTime x { get; set; }
        public double y { get; set; }
        public DataPoint()
        {

        }
    }
}
