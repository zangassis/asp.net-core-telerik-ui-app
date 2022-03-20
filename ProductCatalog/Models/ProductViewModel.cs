namespace ProductCatalog.Models
{
    public class ProductViewModel
    {
        public Guid Id { get; set; }
        public string Displayname { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool Active { get; set; }
        public BrandViewModel Brand { get; set; }
        public ProductPriceViewModel Price { get; set; }
        public CategoryViewModel Category { get; set; }
        public Guid BrandId { get; set; }
    }
}
