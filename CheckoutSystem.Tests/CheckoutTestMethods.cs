namespace CheckoutSystem.Tests
{
    public static class CheckoutTestMethods
    {
        public static List<ItemPrice> GetDefaultPrices()
        {
            return new List<ItemPrice>
            {
                new ItemPrice { Sku = "A", Price = 50 },
                new ItemPrice { Sku = "B", Price = 30 },
                new ItemPrice { Sku = "C", Price = 20 },
                new ItemPrice { Sku = "D", Price = 15 }
            };
        }
        public static List<SpecialOffer> GetDefaultOffers()
        {
            return new List<SpecialOffer>
            {
                new SpecialOffer { ProductSku = "A", RequiredQuantity = 3, OfferPrice = 130 },
                new SpecialOffer { ProductSku = "B", RequiredQuantity = 2, OfferPrice = 45 }
            };
        }
    }    
}