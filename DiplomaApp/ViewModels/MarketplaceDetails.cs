using DiplomaApp.Models;
using X.PagedList;

namespace DiplomaApp.ViewModels
{
    public class MarketplaceDetails
    {
        public Marketplace Marketplace { get; set; }
        public IPagedList <CategoryIndex> Categories { get; set; }

        public MarketplaceDetails()
        {

        }
    }
}
