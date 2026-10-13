namespace MahantInv.Web.Infrastructure.Dtos.Purchase
{
    /// <summary>
    /// Seller of the most recent order for a product, used to prefill the Place Order form.
    /// </summary>
    public class LastSellerDto
    {
        public int SellerId { get; set; }
        public string SellerName { get; set; }
    }
}
