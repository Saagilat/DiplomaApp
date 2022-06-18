namespace DiplomaApp.Models
{
    public class Category
    {
        public Marketplace Marketplace { get; set; }
        public int MarketplaceId { get; set; }
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public DateTime CheckDate { get; set; }
        public string? LastParsedPageUrl { get; set; }

        public ICollection<Offer> Offers = new List<Offer>();
        public Category()
        {
        }
    }
}
