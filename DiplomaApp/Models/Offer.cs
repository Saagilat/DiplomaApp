namespace DiplomaApp.Models
{
    public class Offer
    {
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime CheckDate { get; set; }

        public ICollection<OfferPrice> OfferPrices = new List<OfferPrice>();

        public Offer()
        {
        }
    }

    public class OfferPrice
    {
        public Offer Offer { get; set; }
        public int OfferId { get; set; }
        public float Price { get; set; }
        public DateTime CheckDate { get; set; }


        public OfferPrice()
        {

        }
    }
}
