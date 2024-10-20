namespace CheckoutSystem.Models
{
    public class SpecialOffer 
    {
        public string ProductSku { get; set; } = string.Empty;
        public int RequiredQuantity { get; set; }
        public decimal OfferPrice { get; set; }
    }
}