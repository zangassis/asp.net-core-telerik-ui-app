namespace ProductCatalog.Models
{
    public class ProductPriceViewModel
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public double Price { get; set; }
    }
}
