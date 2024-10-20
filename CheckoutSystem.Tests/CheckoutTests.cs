using System.Data; // for use of "DuplicateNameException"

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
            [Fact]
        public void NonExistingProduct_ThrowsError_WhenSettingOffer()
        {
            var offersToAdd = CheckoutTestMethods.GetDefaultOffers();
            offersToAdd.Add(new SpecialOffer{
                ProductSku = "Z",
                RequiredQuantity = 5,
                OfferPrice = 100M
            });

            var testCheckout = new Checkout();
            
            var exception = Assert.Throws<InvalidOperationException>(() => testCheckout.InitialiseItems(CheckoutTestMethods.GetDefaultPrices(), offersToAdd));
            Assert.Equal("Special Offer Product not found.", exception.Message);
        }
        [Fact]
        public void DuplicateProduct_ThrowsError_WhenSettingPrices()
        {
            var itemsToAdd = CheckoutTestMethods.GetDefaultPrices();
            itemsToAdd.Add(new ItemPrice{
                Sku = "A",
                Price = 1000M
            });

            var testCheckout = new Checkout();

            var exception = Assert.Throws<DuplicateNameException>(() => testCheckout.InitialiseItems(itemsToAdd, null));
            Assert.Equal("A product with the given SKU already exists.", exception.Message);
        }
        [Fact]
        public void DuplicateOffer_ThrowsError_WhenSettingOffers()
        {
            var offersToAdd = CheckoutTestMethods.GetDefaultOffers();
            offersToAdd.Add(new SpecialOffer{
                ProductSku = "A",
                RequiredQuantity = 50,
                OfferPrice = 100M
            });

            var testCheckout = new Checkout();

            var exception = Assert.Throws<DuplicateNameException>(() => testCheckout.InitialiseItems(CheckoutTestMethods.GetDefaultPrices(), offersToAdd));
            Assert.Equal("An offer for the given SKU already exists.", exception.Message);
        }
    }
}