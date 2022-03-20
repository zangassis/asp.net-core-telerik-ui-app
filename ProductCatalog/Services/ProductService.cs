using ProductCatalog.Data.Context;
using ProductCatalog.Models;

namespace ProductCatalog.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<ProductDtoViewModel> GetProducts()
        {
            var products = _dbContext.Products.ToList();
            var categories = _dbContext.Categories.ToList();
            var brands = _dbContext.Brands.ToList();
            var prices = _dbContext.ProductPrices.ToList();
            var productsDto = new List<ProductDtoViewModel>();

            foreach (var product in products)
            {
                var productCategory = categories.Where(p => p.ProductId == product.Id).FirstOrDefault().Name;
                var productBrand = brands.FirstOrDefault(b => b.Id == product.BrandId).Name;
                var productPrice = prices.FirstOrDefault(x => x.ProductId == product.Id).Price;

                var productDto = new ProductDtoViewModel()
                {
                    Id = product.Id,
                    Displayname = product.Displayname,
                    CreationDate = product.CreationDate,
                    LastUpdateDate = product.LastUpdateDate,
                    Price = productPrice,
                    Active = product.Active,
                    Brand = productBrand,
                    Category = productCategory
                };

                productsDto.Add(productDto);
            }
            return productsDto;
        }

        public ProductDtoViewModel GetProductById(string id)
        {
            Guid guidId = Guid.Parse(id);

            var product = _dbContext.Products.FirstOrDefault(p => p.Id == guidId);

            if (product is null)
                return null;

            var productCategory = _dbContext.Categories.Where(p => p.ProductId == guidId).FirstOrDefault().Name;
            var productBrand = _dbContext.Brands.FirstOrDefault(b => b.Id == product.BrandId).Name;
            var productPrice = _dbContext.ProductPrices.FirstOrDefault(x => x.ProductId == guidId).Price;

            return new ProductDtoViewModel()
            {
                Id = product.Id,
                Active = product.Active,
                Brand = productBrand,
                Category = productCategory,
                CreationDate = product.CreationDate,
                Displayname = product.Displayname,
                LastUpdateDate = product.LastUpdateDate,
                Price = productPrice
            };
        }

        public string AddProduct(ProductDtoViewModel productDto)
        {
            Guid id = Guid.NewGuid();

            var product = new ProductViewModel()
            {
                Id = id,
                Displayname = productDto.Displayname,
                Active = true,
                Price = new ProductPriceViewModel() { Id = Guid.NewGuid(), Price = productDto.Price, ProductId = id },
                CreationDate = DateTime.UtcNow,
                LastUpdateDate = DateTime.UtcNow,
                Brand = new BrandViewModel() { Id = Guid.NewGuid(), Name = productDto.Brand },
                Category = new CategoryViewModel() { Id = Guid.NewGuid(), Name = productDto.Category, ProductId = id }
            };

            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();

            return "Create successfully";
        }

        public string UpdateProduct(ProductDtoViewModel productDto)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.Id == productDto.Id);
            var brand = _dbContext.Brands.FirstOrDefault(b => b.Id == product.BrandId);
            var category = _dbContext.Categories.FirstOrDefault(c => c.ProductId == productDto.Id);
            var price = _dbContext.ProductPrices.FirstOrDefault(x => x.ProductId == productDto.Id);

            try
            {
                var updateBrand = new BrandViewModel() { Id = product.BrandId, Name = productDto.Brand };
                var updateCategory = new CategoryViewModel() { Id = category.Id, Name = productDto.Category, ProductId = productDto.Id };
                var updatePrice = new ProductPriceViewModel() { Id = price.Id, Price = productDto.Price, ProductId = productDto.Id };

                _dbContext.Entry(product).CurrentValues.SetValues(productDto);
                _dbContext.Entry(brand).CurrentValues.SetValues(updateBrand);
                _dbContext.Entry(category).CurrentValues.SetValues(updateCategory);
                _dbContext.Entry(price).CurrentValues.SetValues(updatePrice);
                _dbContext.SaveChanges();

                return "Update successfully";
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string DeleteProduct(ProductDtoViewModel productDto)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.Id == productDto.Id);
            if (product is null)
                return "Product not found";

            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();
            return "Delete successfully";
        }

    }
}
