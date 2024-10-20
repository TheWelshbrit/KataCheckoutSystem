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
            scannedItems = new List<ScannedItem>();
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
            if (string.IsNullOrWhiteSpace(item))
            {
                throw new InvalidOperationException("Null or Empty SKU could not be scanned."); 
            }
            if (!itemPrices.Any(x => x.Sku == item))
            {
                throw new InvalidOperationException("A product with the given SKU could not be found."); 
            }
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
            if (scannedItems.Count() == 0)
            {
                throw new InvalidOperationException("Total Price cannot be calculated for an empty basket."); 
            }

            decimal totalPrice = 0;
            foreach (var scannedItem in scannedItems)
            {
                var relevantItem = itemPrices.FirstOrDefault(x => x.Sku == scannedItem.ProductSku);
                var relevantOffer = specialOffers.FirstOrDefault(x => x.ProductSku == scannedItem.ProductSku);
                
                if (relevantOffer != null)
                {
                    var fullSets = (int)(scannedItem.Quantity / relevantOffer.RequiredQuantity);
                    int remainingQuantity = scannedItem.Quantity % relevantOffer.RequiredQuantity;
                    totalPrice += (fullSets * relevantOffer.OfferPrice) + (remainingQuantity * relevantItem.Price);
                }
                else
                {
                    totalPrice += scannedItem.Quantity * relevantItem.Price;
                }
            }
            return (int)totalPrice; 
            // This return cast will convert from decimal to int by truncating the decimal portion of the number, essentially always rounding down.
            // But I'm sure our shop's marketing team can make an advert out of helping our customers to "save their pennies"!
        }
    }
}