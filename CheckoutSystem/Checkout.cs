using CheckoutSystem.Models;
using System.Runtime.CompilerServices;
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
        
        public void Scan(string item)
        {
            return;
        }
        public int GetTotalPrice()
        {
            return 0;
        }
    }
}