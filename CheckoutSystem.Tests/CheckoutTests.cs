namespace CheckoutSystem.Tests
{
    public class CheckoutTests
    {
        [Fact]
        public void Initialised_Prices_AreStored()
        {
            var testCheckout = new Checkout();
            testCheckout.InitialiseItems(CheckoutTestMethods.GetDefaultPrices(), null);

            var expectedItems = CheckoutTestMethods.GetDefaultPrices();

            Assert.NotNull(testCheckout.itemPrices);
            Assert.Equal(expectedItems.Count(), testCheckout.itemPrices.Count());
            foreach (var expectedItem in expectedItems)
            {
                var storedPrice = testCheckout.itemPrices.First(x => x.Sku == expectedItem.Sku);
                Assert.NotNull(storedPrice);
                Assert.Equal(expectedItem.Sku, storedPrice.Sku);
                Assert.Equal(expectedItem.Price, storedPrice.Price);
            }
        }
        [Fact]
        public void Initialised_Offers_AreStored()
        {
            var testCheckout = new Checkout();
            testCheckout.InitialiseItems(CheckoutTestMethods.GetDefaultPrices(), CheckoutTestMethods.GetDefaultOffers());

            var expectedOffers = CheckoutTestMethods.GetDefaultOffers();

            Assert.NotNull(testCheckout.specialOffers);
            Assert.Equal(expectedOffers.Count(), testCheckout.specialOffers.Count());
            foreach (var expectedOffer in expectedOffers)
            {
                var storedOffer = testCheckout.specialOffers.First(x => x.ProductSku == expectedOffer.ProductSku);
                Assert.NotNull(storedOffer);
                Assert.Equal(expectedOffer.ProductSku, storedOffer.ProductSku);
                Assert.Equal(expectedOffer.RequiredQuantity, storedOffer.RequiredQuantity);
                Assert.Equal(expectedOffer.OfferPrice, storedOffer.OfferPrice);
            }
        }
    }
}