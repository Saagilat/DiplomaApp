using DiplomaApp.Models;
using X.PagedList;
using Newtonsoft.Json;

namespace DiplomaApp.ViewModels
{
    public class OfferIndex
    {
        public IPagedList<OfferViewModel> Offers { get; set; }

        public OfferIndex(IPagedList<OfferViewModel> offers)
        {
            Offers = offers;
        }
    }
    public class OfferDetails
    {
        public OfferViewModel Offer { get; set; }

        public List<OfferViewModel> SimilliarOffers = new List<OfferViewModel>();
        public string DatasetsJson { get; set; }
        public List<Dataset> Datasets = new List<Dataset>();
        public OfferDetails(OfferViewModel offer, List<OfferViewModel> similliarOffers)
        {
            Offer = offer;
            var iterator = 1;
            foreach(var offerViewModel in similliarOffers)
            {
                int r = iterator * (150 / similliarOffers.Count());
                int g = 150;
                int b = iterator * (255 / similliarOffers.Count());
                string color = "rgb(" + r + ", " + g + ", " + b + ")";
                string name = offerViewModel.MarketplaceName + " - " + offerViewModel.Name;
                Datasets.Add(new Dataset(offerViewModel.OfferPrices, name, color));
                iterator++;
            }
            DatasetsJson = JsonConvert.SerializeObject(Datasets);
            SimilliarOffers = similliarOffers;
        }

        public class Dataset
        {
            public string label { get; set; }

            public List<DataPoint> data = new List<DataPoint>();
            public string borderColor { get; set; }
            public Dictionary<string, string> trendlineLinear = new Dictionary<string, string>
            {
                {"projection:", "true"},
                {"lineStyle", "dotted" },
                {"width:", "1" }
            };

            public Dataset(List<OfferPriceViewModel> offerPrices, string name, string color)
            {
                label = name;
                foreach (var offerPrice in offerPrices)
                {
                    data.Add(new DataPoint(offerPrice));
                }
                borderColor = color;
            }
        }

        public class DataPoint
        {
            public DateTime x { get; set; }
            public double y { get; set; }
            public DataPoint(OfferPriceViewModel offerPrice)
            {
                x = offerPrice.CheckDate;
                y = offerPrice.Price;
            }
        }
    }
    public class OfferViewModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string CategoryUrl { get; set; }
        public string MarketplaceName { get; set; }
        public string MarketplaceUrl { get; set; }
        public float Price { get; set; }
        public DateTime CheckDate { get; set; }
        public DateTime CreationDate { get; set; }
        public List<OfferPriceViewModel> OfferPrices = new List<OfferPriceViewModel>();

        public OfferViewModel(Offer offer, Marketplace marketplace, Category category, List<OfferPrice> offerPrices)
        {
            Id = offer.Id;
            Url = offer.Url;
            Name = offer.Name;
            CreationDate = offer.CreationDate;
            Price = offer.Price;
            CheckDate = offer.CheckDate;
            MarketplaceName = marketplace.Name;
            MarketplaceUrl = marketplace.Url;
            CategoryName = category.Name;
            CategoryUrl = category.Url;
            foreach(var offerPrice in offerPrices)
            {
                OfferPrices.Add(new OfferPriceViewModel(offerPrice));
            }
        }
        public OfferViewModel(Offer offer, Marketplace marketplace, Category category)
        {
            Id = offer.Id;
            Url = offer.Url;
            Name = offer.Name;
            CreationDate = offer.CreationDate;
            Price = offer.Price;
            CheckDate = offer.CheckDate;
            MarketplaceName = marketplace.Name;
            MarketplaceUrl = marketplace.Url;
            CategoryName = category.Name;
            CategoryUrl = category.Url;
        }
    }
    
    public class OfferPriceViewModel
    {
        public float Price { get; set; }
        public DateTime CheckDate { get; set; }
        public OfferPriceViewModel(OfferPrice offerPrice)
        {
            Price = offerPrice.Price;
            CheckDate = offerPrice.CheckDate;
        }
    }
}
