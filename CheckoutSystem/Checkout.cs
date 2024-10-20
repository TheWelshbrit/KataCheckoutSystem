using CheckoutSystem.Models;
using System.Data; // for use of "DuplicateNameException"
using System.Runtime.CompilerServices; // To allow visibility of internal fields to test methods
[assembly: InternalsVisibleTo("CheckoutSystem.Tests")]

namespace CheckoutSystem
{
    public class Checkout : ICheckout
    {
        internal List<ItemPrice> itemPrices { get; set; }
        internal List<SpecialOffer> specialOffers { get; set; }
        internal List<ScannedItem> scannedItems { get; set; }

        public Checkout()
        {
            // ensure all lists are not null
            itemPrices = new List<ItemPrice>();
            specialOffers = new List<SpecialOffer>();
            scannedItems = new List<ScannedItem>();
        }
        
        public void InitialiseItems(List<ItemPrice> items, List<SpecialOffer> offers)
        {
            itemPrices = new List<ItemPrice>();
            foreach (var item in items)
            {
                if (itemPrices.Any(x => x.Sku == item.Sku))
                {
                    throw new DuplicateNameException("A product with the given SKU already exists.");
                }
                itemPrices.Add(item);
            }
            
            specialOffers = new List<SpecialOffer>();
            foreach(var offer in offers ?? new List<SpecialOffer>())
            {
                if (!itemPrices.Any(x => x.Sku == offer.ProductSku))
                {
                    throw new InvalidOperationException("Special Offer Product not found."); 
                }
                if (specialOffers.Any(x => x.ProductSku == offer.ProductSku))
                {
                    throw new DuplicateNameException("An offer for the given SKU already exists.");
                }
                specialOffers.Add(offer);
            }
        }

        public void Scan(string item)
        {
            var existingScannedItem = scannedItems.FirstOrDefault(x => x.ProductSku == item);
            if (existingScannedItem != null)
            {
                existingScannedItem.Quantity++;
            }
            else
            {
                scannedItems.Add(new ScannedItem{
                    ProductSku = item,
                    Quantity = 1
                });
            }
        }
        
        public int GetTotalPrice()
        {
            return 0;
        }
    }
}