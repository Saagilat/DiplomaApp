﻿using DiplomaApp.Models;

namespace DiplomaApp.ViewModels
{
    public class CategoryDetails
    {
        public Category Category { get; set; }
        public string MarketplaceName { get; set; }
        public string MarketplaceUrl { get; set; }
        public IEnumerable<OfferIndex> Offers { get; set; }

        public CategoryDetails()
        {

        }
    }
}
