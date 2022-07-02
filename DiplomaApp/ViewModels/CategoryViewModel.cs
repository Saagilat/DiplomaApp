using DiplomaApp.Models;
using X.PagedList;

namespace DiplomaApp.ViewModels
{
    public class CategoryIndex
    {
        public IPagedList<CategoryViewModel> Categories { get; set; }
        public CategoryIndex(IPagedList<CategoryViewModel> categories)
        {
            Categories = categories;
        }
    }
    public class CategoryDetails
    {
        public CategoryViewModel Category { get; set; }
        public IPagedList<OfferViewModel> Offers;
        public CategoryDetails(CategoryViewModel category, IPagedList<OfferViewModel> offers)
        { 
            Category = category;
            Offers = offers;
        }
    }
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string MarketplaceName { get; set; }
        public string MarketplaceUrl { get; set; }
        public CategoryViewModel(Category category, Marketplace marketplace)
        {
            Id = category.Id;
            Url = category.Url;
            Name = category.Name;
            MarketplaceName = marketplace.Name;
            MarketplaceUrl = marketplace.Url;
        }
    }

}
