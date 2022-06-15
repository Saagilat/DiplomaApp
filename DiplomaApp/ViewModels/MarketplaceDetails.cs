using DiplomaApp.Models;

namespace DiplomaApp.ViewModels
{
    public class MarketplaceDetails
    {
        public Marketplace Marketplace { get; set; }
        public IEnumerable <Category> Categories { get; set; }

        public MarketplaceDetails()
        {

        }
    }
}
