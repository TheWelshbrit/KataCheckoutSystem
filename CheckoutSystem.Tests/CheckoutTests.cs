using System.Data; // for use of "DuplicateNameException"

namespace CheckoutSystem.Tests
{
    public class CheckoutTests
    {
        #region Price and Offer Initialisation
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
        #endregion

        #region Scan Item
        [Fact]
        public void Scanned_SingleItem_IsAdded_ToScannedItems()
        {
            var testCheckout = CheckoutTestMethods.GetInitialisedCheckout();
            testCheckout.Scan("A");

            Assert.Equal(testCheckout.scannedItems.Count(), 1);
            
            var scannedItem = testCheckout.scannedItems.First();
            Assert.Equal(scannedItem.ProductSku, "A");
            Assert.Equal(scannedItem.Quantity, 1);
        }

        [Fact]
        public void Scanned_ExistingItem_IsNotDuplicated_InScannedItems()
        {
            var testCheckout = CheckoutTestMethods.GetInitialisedCheckout();
            testCheckout.Scan("A");
            testCheckout.Scan("A");

            Assert.Equal(testCheckout.scannedItems.Count(), 1);
            Assert.Equal(testCheckout.scannedItems.First().ProductSku, "A");
        }

        [Fact]
        public void Scanned_ExistingItem_Increments_ScannedItemsQuantity()
        {
            var testCheckout = CheckoutTestMethods.GetInitialisedCheckout();
            testCheckout.Scan("A");
            testCheckout.Scan("A");

            Assert.Equal(testCheckout.scannedItems.Count(), 1);
            var scannedItem = testCheckout.scannedItems.First();
            Assert.Equal(scannedItem.ProductSku, "A");
            Assert.Equal(scannedItem.Quantity, 2);
        }

        [Fact]
        public void Scanned_MultipleItems_AreAdded_ToScannedItems()
        {
            var testCheckout = CheckoutTestMethods.GetInitialisedCheckout();
            testCheckout.Scan("A");
            testCheckout.Scan("B");

            Assert.Equal(testCheckout.scannedItems.Count(), 2);
            Assert.True(testCheckout.scannedItems.Any(x => x.ProductSku == "A"));
            Assert.True(testCheckout.scannedItems.Any(x => x.ProductSku == "B"));
        }

        [Fact] 
        public void Scanned_MultipleItems_AreRecorded_WithCorrectQuantities()
        {
            var testCheckout = CheckoutTestMethods.GetInitialisedCheckout();
            testCheckout.Scan("A");
            testCheckout.Scan("A");
            testCheckout.Scan("B");
            testCheckout.Scan("B");
            testCheckout.Scan("B");

            Assert.Equal(testCheckout.scannedItems.Count(), 2);

            var scannedItemA = testCheckout.scannedItems.FirstOrDefault(x => x.ProductSku == "A");
            var scannedItemB = testCheckout.scannedItems.FirstOrDefault(x => x.ProductSku == "B");
            
            Assert.Equal(scannedItemA.ProductSku, "A");
            Assert.Equal(scannedItemA.Quantity, 2);
            Assert.Equal(scannedItemB.ProductSku, "B");
            Assert.Equal(scannedItemB.Quantity, 3);            
        }

        [Fact]
        public void Scanned_MixedOrderItems_AreRecorded_WithCorrectQuantities()
        {
            var testCheckout = CheckoutTestMethods.GetInitialisedCheckout();

            // 1xA, 2xB, 3xC, 4xD
            testCheckout.Scan("A");
            testCheckout.Scan("B");
            testCheckout.Scan("C");
            testCheckout.Scan("D");
            testCheckout.Scan("B");
            testCheckout.Scan("C");
            testCheckout.Scan("D");
            testCheckout.Scan("C");
            testCheckout.Scan("D");
            testCheckout.Scan("D");

            Assert.Equal(testCheckout.scannedItems.Count(), 4);

            var scannedItemA = testCheckout.scannedItems.FirstOrDefault(x => x.ProductSku == "A");
            var scannedItemB = testCheckout.scannedItems.FirstOrDefault(x => x.ProductSku == "B");
            var scannedItemC = testCheckout.scannedItems.FirstOrDefault(x => x.ProductSku == "C");
            var scannedItemD = testCheckout.scannedItems.FirstOrDefault(x => x.ProductSku == "D");
            
            Assert.Equal(scannedItemA.ProductSku, "A");
            Assert.Equal(scannedItemA.Quantity, 1);
            Assert.Equal(scannedItemB.ProductSku, "B");
            Assert.Equal(scannedItemB.Quantity, 2);
            Assert.Equal(scannedItemC.ProductSku, "C");
            Assert.Equal(scannedItemC.Quantity, 3);
            Assert.Equal(scannedItemD.ProductSku, "D");
            Assert.Equal(scannedItemD.Quantity, 4);
        }

        [Fact]
        public void Scanned_UnrecognisedProduct_ThrowsError()
        {
            var testCheckout = CheckoutTestMethods.GetInitialisedCheckout();

            var exception = Assert.Throws<InvalidOperationException>(() => testCheckout.Scan("Z"));
            Assert.Equal("A product with the given SKU could not be found.", exception.Message);
        }
        
        [Fact]
        public void Scanned_NullSku_ThrowsError()
        {
            var testCheckout = CheckoutTestMethods.GetInitialisedCheckout();

            var exception = Assert.Throws<InvalidOperationException>(() => testCheckout.Scan(null));
            Assert.Equal("Null or Empty SKU could not be scanned.", exception.Message);
        }
         
        [Fact]
        public void Scanned_EmptySku_ThrowsError()
        {
            var testCheckout = CheckoutTestMethods.GetInitialisedCheckout();

            var exception = Assert.Throws<InvalidOperationException>(() => testCheckout.Scan(string.Empty));
            Assert.Equal("Null or Empty SKU could not be scanned.", exception.Message);
        }

        [Fact]
        public void Scanned_WhiteSpaceSku_ThrowsError()
        {
            var testCheckout = CheckoutTestMethods.GetInitialisedCheckout();

            var exception = Assert.Throws<InvalidOperationException>(() => testCheckout.Scan("   "));
            Assert.Equal("Null or Empty SKU could not be scanned.", exception.Message);
        }
        #endregion
    }
}